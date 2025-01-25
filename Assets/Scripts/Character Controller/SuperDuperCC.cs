using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SuperDuperCC : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform lookDirection;
    [SerializeField] private PlayerInput playerInput;

    [Header("Interpolation Settings")]
    [SerializeField] private Transform interpolateTarget;    
    [SerializeField] private float smoothTime = 25f;

    [Header("Movement Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Look Settings")]
    [SerializeField] private float maxLookAngle = 90f;
    
    private bool _jump = false;
    private bool _isJumping = false;
    private Vector3 _move;
    private float _initialJumpVelocity;

    private void Start()
    {
        playerInput.actions["Jump"].performed += ctx => HandleJump();
        _initialJumpVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);

    }

    private void HandleJump()
    {
        if(controller.isGrounded && !_isJumping)
            _jump = true;
    }

    void Update()
    {
        HandleMovement();
        HandleMouse();
        InterpolateVisuals();
    }

    private void HandleMouse()
    {
        var mouseInput = playerInput.actions["Look"].ReadValue<Vector2>();

        if (mouseInput == Vector2.zero) return;

        // Rotate player using mouse input as quaternion delta
        var deltaXAxis = Quaternion.Euler(-mouseInput.y, 0, 0);
        var deltaYAxis = Quaternion.Euler(0, mouseInput.x, 0);
      
        controller.transform.rotation *= deltaYAxis;
        lookDirection.rotation *= deltaXAxis;
    }

    private void HandleMovement()
    {
        var input = playerInput.actions["Move"].ReadValue<Vector2>();
        
        if (controller.isGrounded && _jump && !_isJumping)
        {
            StartCoroutine(JumpCoroutine());
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

        if (_move is { x: 0, z: 0 }) return;
        
        // Rotate player to face movement direction
        var lateralMove = new Vector3(_move.x, 0, _move.z);
        var lookRotation = Quaternion.LookRotation(lateralMove);
        interpolateTarget.rotation = Quaternion.Slerp(interpolateTarget.rotation, lookRotation, Time.deltaTime * smoothTime);
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
