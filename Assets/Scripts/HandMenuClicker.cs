using UnityEngine;
using UnityEngine.InputSystem;

public class HandMenuClicker : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionProperty menuButtonAction;

    // Timer to prevent double-clicks
    private float cooldown = 0f;

    void OnTriggerStay(Collider other)
    {
        // 1. Check if the Menu Button is pressed
        float buttonValue = menuButtonAction.action.ReadValue<float>();

        if (buttonValue > 0.5f && Time.time > cooldown)
        {
            // 2. Find the SceneLoader (The messenger)
            SceneLoader loader = FindFirstObjectByType<SceneLoader>();
            if (loader == null) return;

            // 3. Check which button we are touching
            if (other.CompareTag("HeavenButton"))
            {
                Debug.Log("Touched Heaven!");
                loader.LoadHeavenMode();
                cooldown = Time.time + 1.0f; // Cooldown
            }
            else if (other.CompareTag("HellButton"))
            {
                Debug.Log("Touched Hell!");
                loader.LoadHellMode();
                cooldown = Time.time + 1.0f;
            }
            else if (other.CompareTag("ReturnButton"))
            {
                Debug.Log("Touched Return!");
                loader.LoadMenu();
                cooldown = Time.time + 1.0f;
            }
        }
    }
}