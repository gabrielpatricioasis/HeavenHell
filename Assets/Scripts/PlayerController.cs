using UnityEngine;
using UnityEngine.InputSystem; // Required for the new input system

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    public CharacterController characterController; // We'll use a CharacterController for smooth movement

    [Header("Embodiment Parameters")]
    private float speedMultiplier;
    private float gravityMultiplier;
    private float jumpForce;
    private Vector3 verticalVelocity; // Stores our current jump/fall speed

    [Header("Input Action References")]
    public InputActionReference leftHandMoveAction;
    public InputActionReference rightHandAButtonAction;

    // A simple structure to hold the parameters for each mode
    [System.Serializable]
    public struct EmbodimentSettings
    {
        public float speed;
        public float gravity;
        public float jump;
    }

    [Header("Mode Settings")]
    public EmbodimentSettings heavenParams;
    public EmbodimentSettings hellParams;

    void Awake()
    {
        // Get the CharacterController component attached to this object
        characterController = GetComponent<CharacterController>();

        // Check the GameManager and apply the correct parameters
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.choice == GameManager.PlayerChoice.Heaven)
            {
                ApplySettings(heavenParams);
                Debug.Log("Player Controller set to HEAVEN parameters.");
            }
            else // Default to Hell if not Heaven (or if choice is None)
            {
                ApplySettings(hellParams);
                Debug.Log("Player Controller set to HELL parameters.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager not found. Defaulting to Heaven parameters for testing.");
            ApplySettings(heavenParams);
        }
    }

    void Start()
    {
        // We need to enable the jump action to listen for it
        rightHandAButtonAction.action.performed += OnJump;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        rightHandAButtonAction.action.performed -= OnJump;
    }
    
    // This function copies the chosen settings into our active variables
    void ApplySettings(EmbodimentSettings settings)
    {
        speedMultiplier = settings.speed;
        gravityMultiplier = settings.gravity;
        jumpForce = settings.jump;
    }

    void Update()
    {
        // --- LOCOMOTION ---
        // Read the joystick input from the left controller
        Vector2 joystickValue = leftHandMoveAction.action.ReadValue<Vector2>();

        // Get the direction the user is looking
        Vector3 headDirection = transform.Find("Camera Offset/Main Camera").forward;
        Vector3 moveDirection = new Vector3(joystickValue.x, 0, joystickValue.y);
        moveDirection = Quaternion.LookRotation(new Vector3(headDirection.x, 0, headDirection.z)) * moveDirection;

        // Apply movement
        characterController.Move(moveDirection * speedMultiplier * Time.deltaTime);

        // --- GRAVITY ---
        // If the character is on the ground, stop the downward velocity
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // A small downward force to keep it grounded
        }

        // Apply our custom gravity
        verticalVelocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    // --- JUMP ---
    private void OnJump(InputAction.CallbackContext context)
    {
        // Can only jump if on the ground
        if (characterController.isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * (Physics.gravity.y * gravityMultiplier));
        }
    }
}