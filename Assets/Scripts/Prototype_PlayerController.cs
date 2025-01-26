using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Prototype_PlayerController : MonoBehaviour

{
    [Header("Movement Settings")] public float moveSpeed = 2.0f;
    public float gravity = 9.81f;
    public float jumpHeight = 0.15f;

    [Header("Camera Settings")] public float mouseSensitivityX = 150f;
    public float mouseSensitivityY = 100f;
    public Transform cameraPivot;
    public float verticalRotationLimit = 60f;

    private CharacterController characterController;

    // Track vertical velocity (gravity/jump).
    private float verticalVelocity = 0f;

    // Track vertical camera pitch for clamping.
    private float cameraPitch = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Optional: lock and hide the cursor (hit ESC in Play mode to release).
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        // Rotate the player left/right.
        transform.Rotate(Vector3.up, mouseX);

        // Tilt the camera pivot up/down, clamped.
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalRotationLimit, verticalRotationLimit);
        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        // WASD or arrow keys (old Input Manager).
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        move *= moveSpeed;

        // If grounded, reset or apply jump.
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; // small negative to keep grounded

            // Space = Jump (old input manager).
            if (Input.GetButtonDown("Jump"))
            {
                // Basic v = sqrt(2*g*h).
                verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            }
        }
        else
        {
            // Gravity when not grounded.
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Combine horizontal & vertical.
        move.y = verticalVelocity;
        characterController.Move(move * Time.deltaTime);
    }
}