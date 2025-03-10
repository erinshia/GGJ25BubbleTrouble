using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SuperDuperCC : MonoBehaviour
{
    public static SuperDuperCC Instance;
    
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private LayerMask hitLayer;

    [Header("Interpolation Settings")]
    [SerializeField] private Transform interpolateTarget;    
    [SerializeField] private float smoothTime = 25f;

    [Header("Movement Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSensitivity = 10f;
    [SerializeField] private Transform lookDirection;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private CinemachineCamera freeLookCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private Transform rayCastOrigin;
    
    [Header("Constraints")]
    [SerializeField] private Transform lookAtConstraint;

    public Transform PlayerVisuals => interpolateTarget;
    
    public event Action OnAimEnter;
    public event Action OnAimExit;
    public event Action OnJump;
    
    /// <summary>
    /// Invoked when the player shoots.
    /// The first Vector3 is the origin of the raycast.
    /// The second Vector3 is the direction of the raycast.
    /// </summary>
    public event Action<Vector3, Vector3> OnShoot;
    
    private bool _jump = false;
    private bool _isJumping = false;
    private Vector3 _move;
    private float _initialJumpVelocity;
    private bool _isAiming = false;
    private float _xRotation = 0f; // Tracks the current X-axis rotation
    private bool _isShooting = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        playerInput.actions["Jump"].performed += ctx => HandleJump();
        _initialJumpVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);
        playerInput.actions["Aim"].performed += ctx => HandleAim(true);
        playerInput.actions["Aim"].canceled += ctx => HandleAim(false);
        playerInput.actions["Attack"].performed += ctx => _isShooting = true;
        playerInput.actions["Attack"].canceled += ctx => _isShooting = false;
        HandleAim(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    

    private void HandleAim(bool isAiming)
    {
        _isAiming = isAiming;
        if (_isAiming)
        {
            aimCam.Prioritize();
            OnAimEnter?.Invoke();
        }
        else
        {
            freeLookCam.Prioritize();
            OnAimExit?.Invoke();
        }
    }

    private void HandleJump()
    {
        if (controller.isGrounded && !_isJumping)
        {
            _jump = true;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleMouse();
        HandleShoot();
        InterpolateVisuals();
    }

    private void HandleShoot()
    {
        if (!_isShooting || !_isAiming) return;

        // Create a ray from the center of the screen
        var aimRay = new Ray(aimCam.transform.position, aimCam.transform.forward);

        // Calculate direction from the gun's origin to the aim point (center of the screen)
        if (Physics.Raycast(aimRay, out var hit, 1000f, hitLayer))
        {
            var shootDirection = (hit.point - rayCastOrigin.position).normalized;
            Debug.DrawRay(rayCastOrigin.position, shootDirection * 200f, Color.green);
            OnShoot?.Invoke(rayCastOrigin.position, shootDirection);
        }
        else
        {
            Debug.DrawRay(rayCastOrigin.position, aimRay.direction * 200f, Color.red);
            OnShoot?.Invoke(rayCastOrigin.position, aimRay.direction);
        }
    }

    private void HandleMouse()
    {
        var mouseInput = playerInput.actions["Look"].ReadValue<Vector2>();
        mouseInput *= Time.deltaTime;
        mouseInput *= cameraSensitivity;

        if (mouseInput == Vector2.zero) return;

        // Rotate player using mouse input as quaternion delta
        var deltaYAxis = Quaternion.Euler(0, mouseInput.x, 0);
      
        controller.transform.rotation *= deltaYAxis;
        
        _xRotation -= mouseInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
        lookDirection.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }
    
    private void HandleMovement()
    {
        var input = playerInput.actions["Move"].ReadValue<Vector2>();
        
        if (controller.isGrounded && _jump && !_isJumping)
        {
             StartCoroutine(JumpCoroutine());
            OnJump?.Invoke();
        }
        
        float gravityMultiplier = _move.y < 0 ? 2f : 1f;
        var currentGravity = _move.y + (gravity * gravityMultiplier * Time.deltaTime);
        _move.y = Mathf.Clamp(currentGravity, gravity * mass, _initialJumpVelocity);

        
        var lateralPreviousFrame = new Vector3(_move.x, 0, _move.z);
        var lateralMagnitude = lateralPreviousFrame.magnitude;
        var speedFactor = lateralMagnitude + acceleration * Time.deltaTime;
        
        var forward = controller.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        var right = controller.transform.right;
        right.y = 0;
        right.Normalize();
        
        var lateralCurrentFrame = forward * input.y + right * input.x;
        Debug.DrawRay(controller.transform.position, lateralCurrentFrame.normalized, Color.red);
        
        lateralCurrentFrame = Vector3.ClampMagnitude(lateralCurrentFrame.normalized * speedFactor, maxSpeed);
        
        lateralCurrentFrame.y = 0;
        lateralCurrentFrame = Vector3.ClampMagnitude(lateralCurrentFrame, maxSpeed);
        
        _move = new Vector3(lateralCurrentFrame.x, _move.y, lateralCurrentFrame.z);

        controller.Move(_move * Time.deltaTime);
    }

    private void InterpolateVisuals()
    {
        var targetPosition = controller.transform.position;
        interpolateTarget.position = Vector3.Lerp(interpolateTarget.position, targetPosition, Time.deltaTime * smoothTime);

        if (_isAiming)
        {
            //raycast from cinematic camera
            var ray = new Ray(aimCam.transform.position, aimCam.transform.forward);
            if (Physics.Raycast(ray, out var hit, 1000f))
            {
                var target = hit.point;
                //rotate the player to face the target
                target.y = controller.transform.position.y;
                var lookAtTarget = Quaternion.LookRotation(target - controller.transform.position);
                interpolateTarget.rotation = Quaternion.Slerp(interpolateTarget.rotation, lookAtTarget, Time.deltaTime * smoothTime);
                lookAtConstraint.position = target;
            }
            else
            {
                //rotate the player to face the camera forward
                var lookAtCamera = Quaternion.LookRotation(aimCam.transform.forward);
                interpolateTarget.rotation = Quaternion.Slerp(interpolateTarget.rotation, lookAtCamera, Time.deltaTime * smoothTime);
                lookAtConstraint.position = aimCam.transform.position + aimCam.transform.forward * 100f;
            }
            return;
        }
        
        
        if (_move is { x: 0, z: 0 }) return;
        
        // Rotate player to face movement direction
        var lateralMove = new Vector3(_move.x, 0, _move.z);
        var lookRotation = Quaternion.LookRotation(lateralMove);
        interpolateTarget.rotation = Quaternion.Slerp(interpolateTarget.rotation, lookRotation, Time.deltaTime * smoothTime);
        lookAtConstraint.position = controller.transform.position + lateralMove;
    }

    private IEnumerator JumpCoroutine()
    {
        _isJumping = true;
        _jump = false;

        _initialJumpVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);
        _move.y = _initialJumpVelocity;
        var timer = 0f;
        
        while (timer < (2 * _initialJumpVelocity / -gravity))
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        _isJumping = false;
    }
}
