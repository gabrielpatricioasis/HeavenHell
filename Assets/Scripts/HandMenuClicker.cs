using UnityEngine;
using UnityEngine.InputSystem;

public class HandMenuClicker : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionProperty menuButtonAction;

    // Static flag
    public static bool isHovering = false;
    private float cooldown = 0f;

    // --- FIX 1: FORCE INPUT TO ENABLE ---
    // If this is missing, manual bindings often stay "OFF"
    void OnEnable()
    {
        if (menuButtonAction.action != null) menuButtonAction.action.Enable();
    }
    void OnDisable()
    {
        if (menuButtonAction.action != null) menuButtonAction.action.Disable();
    }
    // ------------------------------------

    void Start()
    {
        isHovering = false;
    }

    // --- DEBUG 1: TEST BUTTON PRESS ANYWHERE ---
    void Update()
    {
        // This will print "Button Pressed!" in the console even if you are NOT touching a button.
        // If you don't see this, your Input Binding is wrong.
        if (menuButtonAction.action != null && menuButtonAction.action.WasPressedThisFrame())
        {
            Debug.Log("TEST: Controller Button was pressed successfully!");
        }
    }

    // --- DEBUG 2: TEST TOUCHING ---
    void OnTriggerEnter(Collider other)
    {
        if (IsValidButton(other))
        {
            isHovering = true;
            Debug.Log($"PHYSICS: Hand touched {other.gameObject.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidButton(other))
        {
            isHovering = false;
            Debug.Log($"PHYSICS: Hand left {other.gameObject.name}");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsValidButton(other)) return;

        isHovering = true;

        // Check Input
        float buttonValue = menuButtonAction.action.ReadValue<float>();

        // Note: Using ReadValue > 0.5 is safer for buttons than WasPressedThisFrame in Stay loops
        if (buttonValue > 0.5f && Time.time > cooldown)
        {
            Debug.Log("LOGIC: Touching Button AND Pressing Input -> Executing!");

            SceneLoader loader = FindFirstObjectByType<SceneLoader>();
            if (loader == null)
            {
                Debug.LogError("CRITICAL: SceneLoader not found!");
                return;
            }

            if (other.CompareTag("HeavenButton"))
            {
                loader.LoadHeavenMode();
                cooldown = Time.time + 2.0f;
            }
            else if (other.CompareTag("HellButton"))
            {
                loader.LoadHellMode();
                cooldown = Time.time + 2.0f;
            }
            else if (other.CompareTag("ReturnButton"))
            {
                other.gameObject.SetActive(false);
                isHovering = false;
                loader.LoadMenu();
                cooldown = Time.time + 2.0f;
            }
        }
    }

    bool IsValidButton(Collider col)
    {
        return col.CompareTag("ReturnButton") || col.CompareTag("HeavenButton") || col.CompareTag("HellButton");
    }
}