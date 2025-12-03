using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Current LogInt Parameters")]
    // Valores por defecto (se sobrescriben al iniciar según el mundo)
    public float currentSpeedMultiplier = 80.0f;
    public float currentGravityMultiplier = 0.5f;
    public float currentJumpForce = 20.0f;

    // --- NUEVO: Velocidad de giro ---
    public float turnSpeed = 60.0f;

    [Header("VR Component References")]
    public Transform leftHandCtrl;
    public Transform rightHandCtrl;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionReference moveAction;        // XRI Left Locomotion/Move
    public InputActionReference turnAction;        // XRI Right Locomotion/Turn (NUEVO)
    public InputActionReference jumpAction;        // XRI Right Interaction/Select (Button A)
    public InputActionReference leftTriggerAction; // XRI Left Interaction/Activate
    public InputActionReference rightTriggerAction;// XRI Right Interaction/Activate

    [Header("Visuals")]
    public LineRenderer leftLaser;  // Arrastra el LineRenderer de la mano Izq
    public LineRenderer rightLaser; // Arrastra el LineRenderer de la mano Der

    // Variables privadas
    private InteractableEnergyStream currentActiveStream = null;
    private Vector3 verticalVelocity;

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        // Inicializamos los parámetros según el mundo actual
        if (WorldManager.Instance != null)
            UpdateLogIntParameters(WorldManager.Instance.currentState);
    }

    void Update()
    {
        HandleLocomotion();
        HandleRotation(); // <--- NUEVO: Función de rotación
        HandleJump();
        CheckIfFallen();

        // Función unificada para Monolitos y Energía
        HandleShootingInteraction();
    }

    // --- CONFIGURACIÓN DE MUNDOS (VELOCIDADES) ---
    public void UpdateLogIntParameters(WorldManager.WorldState newState)
    {
        if (newState == WorldManager.WorldState.Heaven)
        {
            currentSpeedMultiplier = 80.0f; // MUY RÁPIDO
            currentGravityMultiplier = 0.5f; // Flotante
            currentJumpForce = 20.0f;        // Salto alto
        }
        else
        {
            currentSpeedMultiplier = 15.0f; // MUY LENTO (Pesado)
            currentGravityMultiplier = 2.0f; // Gravedad fuerte
            currentJumpForce = 3.0f;        // Salto apenas perceptible
        }
    }

    // --- MOVIMIENTO ---
    void HandleLocomotion()
    {
        Vector2 input = Vector2.zero;
        if (moveAction != null) input = moveAction.action.ReadValue<Vector2>();

        // Soporte para Teclado (WASD) si no hay mando
        if (input == Vector2.zero)
        {
            if (Input.GetKey(KeyCode.W)) input.y = 1;
            if (Input.GetKey(KeyCode.S)) input.y = -1;
            if (Input.GetKey(KeyCode.A)) input.x = -1;
            if (Input.GetKey(KeyCode.D)) input.x = 1;
        }

        // Fix Turbo (Normalizar si hay input mínimo)
        if (input.magnitude > 0.1f) input.Normalize();

        // Direcciones planas (Ignorar altura para no frenarse mirando al suelo)
        Vector3 forwardFlat = transform.forward; forwardFlat.y = 0; forwardFlat.Normalize();
        Vector3 rightFlat = transform.right; rightFlat.y = 0; rightFlat.Normalize();
        Vector3 moveDir = rightFlat * input.x + forwardFlat * input.y;

        // MOVIMIENTO REAL (Usamos la variable directa, sin seguros)
        characterController.Move(moveDir * currentSpeedMultiplier * Time.deltaTime);

        // GRAVEDAD
        if (characterController.isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;
        verticalVelocity.y += Physics.gravity.y * currentGravityMultiplier * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    // --- NUEVO: ROTACIÓN (Giro) ---
    void HandleRotation()
    {
        float turnInput = 0f;

        // 1. Leer Joystick Derecho
        if (turnAction != null)
        {
            Vector2 joystickVal = turnAction.action.ReadValue<Vector2>();
            turnInput = joystickVal.x; // Solo nos importa izq/der
        }

        // 2. Soporte Teclado (Q/E o Flechas)
        if (Mathf.Abs(turnInput) < 0.1f)
        {
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow)) turnInput = 1f;
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) turnInput = -1f;
        }

        // 3. Aplicar rotación al personaje
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
        }
    }

    // --- SALTO ---
    void HandleJump()
    {
        // 1. SEGURIDAD: ¿Estás disparando con el gatillo derecho?
        float triggerValue = 0f;
        if (rightTriggerAction != null) triggerValue = rightTriggerAction.action.ReadValue<float>();

        // BLOQUEO: Si aprietas gatillo, PROHIBIDO SALTAR
        if (triggerValue > 0.1f) return;

        // 2. DETECTAR SALTO (Botón A / Grip)
        bool jumpPressed = false;
        if (jumpAction != null) jumpPressed = jumpAction.action.WasPerformedThisFrame();

        // Soporte Tecla Espacio (PC)
        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;

        // 3. EJECUTAR SALTO
        if (jumpPressed && characterController.isGrounded)
        {
            // Usamos la fuerza configurada en UpdateLogIntParameters
            verticalVelocity.y = Mathf.Sqrt(currentJumpForce * -2f * (Physics.gravity.y * currentGravityMultiplier));
        }
    }

    // --- DISPARO Y LÁSER (Monolitos + Energía) ---
    void HandleShootingInteraction()
    {
        // Leemos los Triggers
        float leftValue = (leftTriggerAction != null) ? leftTriggerAction.action.ReadValue<float>() : 0;
        float rightValue = (rightTriggerAction != null) ? rightTriggerAction.action.ReadValue<float>() : 0;

        // Teclas de PC (T o P disparan)
        bool pcShoot = Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.P);

        // MANO IZQUIERDA
        if (leftValue > 0.1f)
        {
            FireRay(leftHandCtrl, leftLaser);
        }
        else if (leftLaser != null) leftLaser.enabled = false;

        // MANO DERECHA
        if (rightValue > 0.1f)
        {
            FireRay(rightHandCtrl, rightLaser);
        }
        else if (rightLaser != null) rightLaser.enabled = false;

        // MODO PC (Cámara)
        if (pcShoot)
        {
            FireRay(Camera.main.transform, null);
        }

        // SOLTAR ENERGY STREAM (Si no se aprieta nada)
        if (leftValue <= 0.1f && rightValue <= 0.1f && !pcShoot && currentActiveStream != null)
        {
            currentActiveStream.StopTriggerHold();
            currentActiveStream = null;
        }
    }

    // Lógica del Raycast
    void FireRay(Transform origin, LineRenderer laser)
    {
        // DISTANCIA AUMENTADA A 500 METROS
        float maxDist = 500f;

        // Dibujar Láser
        if (laser != null)
        {
            laser.enabled = true;
            laser.SetPosition(0, origin.position);
            laser.SetPosition(1, origin.position + origin.forward * maxDist);
        }

        RaycastHit hit;
        if (Physics.Raycast(origin.position, origin.forward, out hit, maxDist))
        {
            // Cortar láser visual donde choque
            if (laser != null) laser.SetPosition(1, hit.point);

            // 1. MONOLITOS (Crecer/Explotar)
            InteractableMonolith monolith = hit.collider.GetComponent<InteractableMonolith>();
            if (monolith != null)
            {
                monolith.ReceivePush();
            }

            // 2. ENERGY STREAMS (Activar sonido/partículas)
            InteractableEnergyStream stream = hit.collider.GetComponent<InteractableEnergyStream>();
            if (stream != null)
            {
                if (currentActiveStream != stream)
                {
                    if (currentActiveStream != null) currentActiveStream.StopTriggerHold();
                    currentActiveStream = stream;
                }
                currentActiveStream.StartTriggerHold();
            }
        }
    }

    // --- SEGURIDAD: RESPAWN SI CAES ---
    void CheckIfFallen()
    {
        if (transform.position.y < -10f)
        {
            characterController.enabled = false;
            transform.position = new Vector3(0, 2, 0); // Vuelta al centro
            verticalVelocity = Vector3.zero;
            characterController.enabled = true;
        }
    }
}