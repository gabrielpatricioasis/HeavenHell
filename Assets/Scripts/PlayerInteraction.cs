using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Current LogInt Parameters")]
    public float currentSpeedMultiplier = 50.0f;
    public float currentGravityMultiplier = 0.5f;
    public float currentJumpForce = 10.0f;

    [Header("VR Component References")]
    public Transform leftHandCtrl;
    public Transform rightHandCtrl;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference leftTriggerAction;
    public InputActionReference rightTriggerAction;

    [Header("Visuals - ¡NUEVO!")]
    public LineRenderer leftLaser;  // Arrastra el LineRenderer de la mano Izq
    public LineRenderer rightLaser; // Arrastra el LineRenderer de la mano Der

    private InteractableEnergyStream currentActiveStream = null;
    private Vector3 verticalVelocity;

    void Start()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (WorldManager.Instance != null) UpdateLogIntParameters(WorldManager.Instance.currentState);
    }

    void Update()
    {
        HandleLocomotion();
        HandleJump();
        CheckIfFallen();

        // ESTA ES LA FUNCIÓN NUEVA QUE HACE TODO (Monolito + Energia)
        HandleShootingInteraction();
    }

    // --- FUNCIÓN UNIFICADA: APUNTAR Y DISPARAR ---
    void HandleShootingInteraction()
    {
        // 1. Leemos los Triggers
        float leftValue = (leftTriggerAction != null) ? leftTriggerAction.action.ReadValue<float>() : 0;
        float rightValue = (rightTriggerAction != null) ? rightTriggerAction.action.ReadValue<float>() : 0;

        // Teclas de PC para emergencias (T o P disparan igual)
        bool pcShoot = Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.P);

        // --- MANO IZQUIERDA ---
        if (leftValue > 0.1f)
        {
            FireRay(leftHandCtrl, leftLaser);
        }
        else
        {
            if (leftLaser != null) leftLaser.enabled = false; // Apagar láser si no disparas
        }

        // --- MANO DERECHA ---
        if (rightValue > 0.1f)
        {
            FireRay(rightHandCtrl, rightLaser);
        }
        else
        {
            if (rightLaser != null) rightLaser.enabled = false; // Apagar láser si no disparas
        }

        // --- MODO PC (Cámara) ---
        if (pcShoot)
        {
            FireRay(Camera.main.transform, null);
        }

        // LÓGICA DE SOLTAR (Para el Energy Stream)
        // Si no aprietas nada, soltamos el stream activo
        if (leftValue <= 0.1f && rightValue <= 0.1f && !pcShoot && currentActiveStream != null)
        {
            currentActiveStream.StopTriggerHold();
            currentActiveStream = null;
        }
    }

    // Dispara el rayo y decide qué hacer
    void FireRay(Transform origin, LineRenderer laser)
    {
        // Activar Láser Visual
        if (laser != null)
        {
            laser.enabled = true;
            laser.SetPosition(0, origin.position); // Inicio en la mano
            laser.SetPosition(1, origin.position + origin.forward * 50f); // Final lejos (por defecto)
        }

        RaycastHit hit;
        // Lanzamos rayo a 100 metros
        if (Physics.Raycast(origin.position, origin.forward, out hit, 100f))
        {
            // Cortar el láser visual donde choque
            if (laser != null) laser.SetPosition(1, hit.point);

            // 1. ¿Es un MONOLITO?
            InteractableMonolith monolith = hit.collider.GetComponent<InteractableMonolith>();
            if (monolith != null)
            {
                monolith.ReceivePush(); // ¡CRECER / EXPLOTAR!
            }

            // 2. ¿Es un ENERGY STREAM?
            InteractableEnergyStream stream = hit.collider.GetComponent<InteractableEnergyStream>();
            if (stream != null)
            {
                // Gestionar cambio de stream
                if (currentActiveStream != stream)
                {
                    if (currentActiveStream != null) currentActiveStream.StopTriggerHold();
                    currentActiveStream = stream;
                }
                currentActiveStream.StartTriggerHold(); // ¡SONIDO / COLOR!
            }
        }
    }

    // --- (RESTO DE TU CÓDIGO QUE YA FUNCIONABA) ---
    void CheckIfFallen()
    {
        if (transform.position.y < -10f)
        {
            characterController.enabled = false;
            transform.position = new Vector3(0, 2, 0);
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

        if (input == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.W)) input.y = 1;
            if (Input.GetKey(KeyCode.S)) input.y = -1;
            if (Input.GetKey(KeyCode.A)) input.x = -1;
            if (Input.GetKey(KeyCode.D)) input.x = 1;
        }

        if (input.magnitude > 0.1f) input.Normalize();

        Vector3 forwardFlat = transform.forward; forwardFlat.y = 0; forwardFlat.Normalize();
        Vector3 rightFlat = transform.right; rightFlat.y = 0; rightFlat.Normalize();
        Vector3 moveDir = rightFlat * input.x + forwardFlat * input.y;

        float speed = (currentSpeedMultiplier > 10) ? currentSpeedMultiplier : 50f;
        characterController.Move(moveDir * speed * Time.deltaTime);

        if (characterController.isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;
        verticalVelocity.y += Physics.gravity.y * currentGravityMultiplier * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    void HandleJump()
    {
        // 1. SEGURIDAD: ¿Estás disparando con el gatillo?
        float triggerValue = 0f;
        if (rightTriggerAction != null) triggerValue = rightTriggerAction.action.ReadValue<float>();

        // Si aprietas el gatillo más de un 10%, PROHIBIDO SALTAR.
        // Esto soluciona que salgas volando al disparar el láser.
        if (triggerValue > 0.1f) return;

        // 2. DETECTAR SALTO (Botón A)
        bool jumpPressed = false;
        if (jumpAction != null) jumpPressed = jumpAction.action.WasPerformedThisFrame();

        // Tecla Espacio (PC)
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        // 3. EJECUTAR SALTO (Bajito)
        if (jumpPressed && characterController.isGrounded)
        {
            // Fuerza hardcodeada a 2.0f para que no saltes al espacio exterior
            float jumpForce = 2.0f;

            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * (Physics.gravity.y * currentGravityMultiplier));
        }
    }
}