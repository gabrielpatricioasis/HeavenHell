using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Current LogInt Parameters")]
    public float currentSpeedMultiplier = 50.0f; 
    public float currentGravityMultiplier = 0.5f;
    public float currentJumpForce = 10.0f;
    public float distPush = 0.5f; // Distance from head to hand to trigger Push

    [Header("VR Component References")]
    public Transform leftHandCtrl;
    public Transform rightHandCtrl;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    // We removed the trigger actions because the Energy Stream script handles them now.

    private Vector3 verticalVelocity;

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (WorldManager.Instance != null)
            UpdateLogIntParameters(WorldManager.Instance.currentState);
    }

    void Update()
    {
        HandleLocomotion();
        HandleJump();
        HandlePushingGesture();
        HandleTriggerRaycast();

        // --- AQUÍ ESTÁ LA LLAMADA QUE FALTABA ---
        CheckIfFallen();
    }

    // --- ESTA ES LA FUNCIÓN QUE FALTABA ---
    void CheckIfFallen()
    {
        // Si bajas de altura -10 (caída al vacío)
        if (transform.position.y < -10f)
        {
            // Apagamos el controller un momento para moverlo sin fisicas
            characterController.enabled = false;

            // Te devuelve al centro (0, 2, 0)
            transform.position = new Vector3(0, 2, 0);

            // Resetea la velocidad de caída
            verticalVelocity = Vector3.zero;

            characterController.enabled = true;
            Debug.Log("¡Te caíste! Respawn al centro.");
        }
    }

    public void UpdateLogIntParameters(WorldManager.WorldState newState)
    {
        if (newState == WorldManager.WorldState.Heaven)
        {
            currentSpeedMultiplier = 50.0f;
            currentGravityMultiplier = 0.5f;
            currentJumpForce = 10.0f;
        }
        else
        {
            currentSpeedMultiplier = 20.0f;
            currentGravityMultiplier = 2.0f;
            currentJumpForce = 2.0f;
        }
    }

    void HandleLocomotion()
    {
        Vector2 input = Vector2.zero;
        if (moveAction != null) input = moveAction.action.ReadValue<Vector2>();

        // Keyboard Fallback
        if (input == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.W)) input.y = 1;
            if (Input.GetKey(KeyCode.S)) input.y = -1;
            if (Input.GetKey(KeyCode.A)) input.x = -1;
            if (Input.GetKey(KeyCode.D)) input.x = 1;
        }

        // Camera-Relative Movement (Fixes the "Backward" bug)
        Transform cameraTransform = Camera.main.transform;
        Vector3 forwardFlat = cameraTransform.forward;
        forwardFlat.y = 0; forwardFlat.Normalize();
        Vector3 rightFlat = cameraTransform.right;
        rightFlat.y = 0; rightFlat.Normalize();

        Vector3 moveDir = rightFlat * input.x + forwardFlat * input.y;

        characterController.Move(moveDir * currentSpeedMultiplier * Time.deltaTime);

        // Gravity
        if (characterController.isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;
        verticalVelocity.y += Physics.gravity.y * currentGravityMultiplier * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        bool jumpPressed = false;
        if (jumpAction != null) jumpPressed = jumpAction.action.WasPerformedThisFrame();
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        if (jumpPressed && characterController.isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(currentJumpForce * -2f * (Physics.gravity.y * currentGravityMultiplier));
        }
    }

    void HandlePushingGesture()
    {
        // 1. Check Distance from Head to Hands
        float distLeft = Vector3.Distance(Camera.main.transform.position, leftHandCtrl.position);
        float distRight = Vector3.Distance(Camera.main.transform.position, rightHandCtrl.position);

        // 2. Both arms must be extended
        if (distLeft > distPush && distRight > distPush)
        {
            // 3. Find Midpoint
            Vector3 midPoint = (leftHandCtrl.position + rightHandCtrl.position) / 2;

            // 4. Blast Radius Check
            Collider[] hits = Physics.OverlapSphere(midPoint, 2.0f); // 2 meter blast
            foreach (var hit in hits)
            {
                InteractableMonolith mono = hit.GetComponent<InteractableMonolith>();
                if (mono != null)
                {
                    mono.ReceivePush(); // Triggers Grow/Shatter
                }
            }
        }
    }
}