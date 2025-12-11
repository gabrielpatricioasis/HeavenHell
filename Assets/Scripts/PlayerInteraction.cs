using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    [Header("PC Settings")]
    public float currentSpeedMultiplier = 12.0f;
    public float currentGravityMultiplier = 1.0f;
    public float currentJumpForce = 8.0f;
    public float interactionDistance = 50.0f;

    [Header("Safety Settings")]
    public float minDistanceToInteract = 3.0f;

    [Header("Menu Settings")]
    public string startSceneName = "Start Screen";

    [Header("References")]
    public CharacterController characterController;
    public LineRenderer laserLine;
    public Transform laserOrigin;

    private Vector3 verticalVelocity; // Velocidad de caída/salto
    private WorldManager.WorldState lastKnownState;

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.startWidth = 0.02f;
            laserLine.endWidth = 0.02f;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.material.color = Color.white;
        }

        if (WorldManager.Instance != null)
        {
            lastKnownState = WorldManager.Instance.currentState;
            UpdateLogIntParameters(lastKnownState);
        }
    }

    void Update()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.currentState != lastKnownState)
        {
            lastKnownState = WorldManager.Instance.currentState;
            UpdateLogIntParameters(lastKnownState);
        }

        HandleMovementAndJump(); // FÍSICAS UNIFICADAS
        HandleInteractionsAndLaser();
        CheckRespawn();
        CheckReturnToMenu();
    }

    public void UpdateLogIntParameters(WorldManager.WorldState newState)
    {
        if (newState == WorldManager.WorldState.Heaven)
        {
            currentSpeedMultiplier = 40.0f;
            currentGravityMultiplier = 0.35f;
            currentJumpForce = 18.0f;
        }
        else
        {
            // HELL: Más fuerza para vencer la gravedad pesada
            currentSpeedMultiplier = 20.0f;
            currentGravityMultiplier = 3.0f;
            currentJumpForce = 20.0f; // Subido un poco más para que responda mejor
        }

        if (laserLine != null)
        {
            laserLine.startColor = Color.white;
            laserLine.endColor = new Color(1, 1, 1, 0.5f);
        }
    }

    // --- AQUÍ ESTÁ EL ARREGLO DEL SALTO ---
    void HandleMovementAndJump()
    {
        // 1. Calcular Movimiento Horizontal (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // 2. Gestionar la Gravedad y el Suelo ANTES de saltar
        if (characterController.isGrounded)
        {
            // Si estamos en el suelo y cayendo, reseteamos la velocidad a algo pequeño
            // para mantenernos pegados, pero no acumulamos velocidad infinita.
            if (verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }

            // 3. SALTO (Solo si estamos en el suelo)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // La fórmula física mágica
                verticalVelocity.y = Mathf.Sqrt(currentJumpForce * -2f * (Physics.gravity.y * currentGravityMultiplier));
            }
        }

        // 4. Aplicar Gravedad (Aumenta la velocidad de caída cada frame)
        verticalVelocity.y += Physics.gravity.y * currentGravityMultiplier * Time.deltaTime;

        // 5. MOVIMIENTO FINAL (UNIFICADO)
        // Movemos el personaje una sola vez sumando (Horizontal * Velocidad) + (Vertical)
        Vector3 finalMovement = (move * currentSpeedMultiplier) + verticalVelocity;

        characterController.Move(finalMovement * Time.deltaTime);
    }

    void HandleInteractionsAndLaser()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            targetPoint = hit.point - (ray.direction * 0.2f);
            float distanceToObject = hit.distance;

            if (Input.GetMouseButtonDown(0))
            {
                if (distanceToObject > minDistanceToInteract)
                {
                    var monolith = hit.collider.GetComponent<InteractableMonolith>();
                    if (monolith != null) monolith.ReceivePush();
                }
            }

            if (Input.GetMouseButton(1))
            {
                var stream = hit.collider.GetComponent<InteractableEnergyStream>();
                if (stream != null) stream.StartTriggerHold();
            }
        }
        else
        {
            targetPoint = ray.origin + ray.direction * interactionDistance;
        }

        if (laserLine != null && laserOrigin != null)
        {
            laserLine.SetPosition(0, laserOrigin.position);
            laserLine.SetPosition(1, targetPoint);
        }

        if (Input.GetMouseButtonUp(1))
        {
            var allStreams = FindObjectsByType<InteractableEnergyStream>(FindObjectsSortMode.None);
            foreach (var s in allStreams) s.StopTriggerHold();
        }
    }

    void CheckRespawn()
    {
        if (transform.position.y < -10)
        {
            characterController.enabled = false;
            transform.position = new Vector3(0, 2, 0);
            characterController.enabled = true;
            verticalVelocity = Vector3.zero; // Importante resetear la caída al renacer
        }
    }

    void CheckReturnToMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (WorldManager.Instance != null) Destroy(WorldManager.Instance.gameObject);
            if (GameManager.Instance != null) Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene(startSceneName);
        }
    }
}