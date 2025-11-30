using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // --- VIRTUAL SUBJECTIVENESS PARAMETERS (LogInt) ---
    [Header("Current LogInt Parameters")]
    public float currentSpeedMultiplier;
    public float currentGravityMultiplier;
    public float currentJumpForce;
    public float distPush = 0.5f;

    // --- COMPONENT REFERENCES ---
    [Header("VR Component References")]
    public Transform leftHandCtrl;
    public Transform rightHandCtrl;
    public CharacterController characterController;

    // --- PHYSICAL INPUTS (Phint) ---
    [Header("Input Settings")]
    public KeyCode jumpButton = KeyCode.JoystickButton3;
    public string leftTriggerAxis = "Oculus_CrossPlatform_PrimaryIndexTrigger";
    public string rightTriggerAxis = "Oculus_CrossPlatform_SecondaryIndexTrigger";
    public string leftJoystickX = "Oculus_CrossPlatform_PrimaryThumbstickHorizontal";
    public string leftJoystickY = "Oculus_CrossPlatform_PrimaryThumbstickVertical";


    void Start()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        HandleLocomotion();
        HandleJump();
        HandlePushingGesture();
        HandleTriggerHold();
    }

    public void UpdateLogIntParameters(WorldManager.WorldState newState)
    {
        if (newState == WorldManager.WorldState.Heaven)
        {
            // The Creator (Heaven)
            currentSpeedMultiplier = 3.5f;
            currentGravityMultiplier = 0.5f;
            currentJumpForce = 10.0f;
            Debug.Log("LogInt updated to: The Creator");
        }
        else
        {
            // The Corruptor (Hell)
            currentSpeedMultiplier = 0.5f;
            currentGravityMultiplier = 2.0f;
            currentJumpForce = 2.0f;
            Debug.Log("LogInt updated to: The Corruptor");
        }
    }

    void HandleLocomotion()
    {
        float horizontalInput = Input.GetAxis(leftJoystickX);
        float verticalInput = Input.GetAxis(leftJoystickY);

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

        characterController.Move(moveDirection * currentSpeedMultiplier * Time.deltaTime);

        Vector3 gravityVector = Vector3.down * (Physics.gravity.magnitude * currentGravityMultiplier * Time.deltaTime);
        characterController.Move(gravityVector);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpButton) && characterController.isGrounded)
        {
            Debug.Log("Jump initiated with force: " + currentJumpForce);
        }
    }

    void HandlePushingGesture()
    {
        float distLeft = Vector3.Distance(transform.position, leftHandCtrl.position);
        float distRight = Vector3.Distance(transform.position, rightHandCtrl.position);

        if (distLeft > distPush && distRight > distPush)
        {
            Vector3 rayStart = transform.position;
            Vector3 rayDirection = transform.forward;
            RaycastHit hit;

            if (Physics.Raycast(rayStart, rayDirection, out hit, 1.0f))
            {
                InteractableMonolith monolith = hit.collider.GetComponent<InteractableMonolith>();
                if (monolith != null)
                {
                    monolith.ReceivePush();
                }
            }
        }
    }

    void HandleTriggerHold()
    {
        bool leftTriggerActive = Input.GetAxis(leftTriggerAxis) > 0.1f;
        bool rightTriggerActive = Input.GetAxis(rightTriggerAxis) > 0.1f;

        if (leftTriggerActive)
        {
            CheckHandCollision(leftHandCtrl);
        }

        if (rightTriggerActive)
        {
            CheckHandCollision(rightHandCtrl);
        }
    }

    void CheckHandCollision(Transform handTransform)
    {
        Collider[] hitColliders = Physics.OverlapSphere(handTransform.position, 0.1f);

        foreach (var hitCollider in hitColliders)
        {
            InteractableEnergyStream stream = hitCollider.GetComponent<InteractableEnergyStream>();

            if (stream != null)
            {
                stream.StartTriggerHold();
            }
        }
    }
}