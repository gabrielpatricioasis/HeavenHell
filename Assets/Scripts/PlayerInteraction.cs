using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Current LogInt Parameters")]
    public float currentSpeedMultiplier = 50.0f; // Velocidad alta por defecto
    public float currentGravityMultiplier = 0.5f;
    public float currentJumpForce = 10.0f;
    public float distPush = 0.1f; // Distancia sensible para el gesto

    [Header("VR Component References")]
    public Transform leftHandCtrl;
    public Transform rightHandCtrl;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference leftTriggerAction;
    public InputActionReference rightTriggerAction;

    private InteractableEnergyStream currentActiveStream = null;
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

        // Fallback Teclado (WASD)
        if (input == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.W)) input.y = 1;
            if (Input.GetKey(KeyCode.S)) input.y = -1;
            if (Input.GetKey(KeyCode.A)) input.x = -1;
            if (Input.GetKey(KeyCode.D)) input.x = 1;
        }

        if (input.magnitude > 0.1f) input.Normalize();

        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0; forwardFlat.Normalize();
        Vector3 rightFlat = transform.right;
        rightFlat.y = 0; rightFlat.Normalize();

        Vector3 moveDir = rightFlat * input.x + forwardFlat * input.y;

        float speed = (currentSpeedMultiplier > 10) ? currentSpeedMultiplier : 50f;

        characterController.Move(moveDir * speed * Time.deltaTime);

        if (characterController.isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;
        verticalVelocity.y += Physics.gravity.y * currentGravityMultiplier * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        bool jumpPressed = false;
        if (jumpAction != null) jumpPressed = jumpAction.action.WasPerformedThisFrame();

        if (Input.GetKeyDown(KeyCode.Space) && !jumpPressed) jumpPressed = true;

        if (jumpPressed && characterController.isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(currentJumpForce * -2f * (Physics.gravity.y * currentGravityMultiplier));
        }
    }

    void HandlePushingGesture()
    {
        bool gestureDetected = false;

        float distLeft = Vector3.Distance(transform.position, leftHandCtrl.position);
        float distRight = Vector3.Distance(transform.position, rightHandCtrl.position);

        if (distLeft > distPush && distRight > distPush) gestureDetected = true;
        if (Input.GetKeyDown(KeyCode.P)) gestureDetected = true;

        if (gestureDetected)
        {
            RaycastHit hit;
            Transform origin = Input.GetKey(KeyCode.P) ? Camera.main.transform : transform;

            // Raycast a 100 metros
            if (Physics.Raycast(origin.position, origin.forward, out hit, 100.0f))
            {
                InteractableMonolith monolith = hit.collider.GetComponent<InteractableMonolith>();
                if (monolith != null)
                {
                    monolith.ReceivePush();
                }
            }
        }
    }

    void HandleTriggerRaycast()
    {
        float leftValue = (leftTriggerAction != null) ? leftTriggerAction.action.ReadValue<float>() : 0;
        float rightValue = (rightTriggerAction != null) ? rightTriggerAction.action.ReadValue<float>() : 0;

        bool isTriggerHeld = (leftValue > 0.1f || rightValue > 0.1f || Input.GetKey(KeyCode.T));

        InteractableEnergyStream detectedStream = null;

        if (isTriggerHeld)
        {
            RaycastHit hit;
            if (Input.GetKey(KeyCode.T))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f))
                {
                    detectedStream = hit.collider.GetComponent<InteractableEnergyStream>();
                }
            }
            else
            {
                if (leftValue > 0.1f && Physics.Raycast(leftHandCtrl.position, leftHandCtrl.forward, out hit, 100f))
                    detectedStream = hit.collider.GetComponent<InteractableEnergyStream>();

                if (detectedStream == null && rightValue > 0.1f && Physics.Raycast(rightHandCtrl.position, rightHandCtrl.forward, out hit, 100f))
                    detectedStream = hit.collider.GetComponent<InteractableEnergyStream>();
            }
        }

        if (detectedStream != null)
        {
            if (currentActiveStream != detectedStream)
            {
                if (currentActiveStream != null) currentActiveStream.StopTriggerHold();
                currentActiveStream = detectedStream;
            }
            currentActiveStream.StartTriggerHold();
        }
        else if (currentActiveStream != null)
        {
            currentActiveStream.StopTriggerHold();
            currentActiveStream = null;
        }
    }
}