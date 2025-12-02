using UnityEngine;
using UnityEngine.InputSystem;

public class HandMenuClicker : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("Assign 'XR Controller (Left Hand) > Optional > menu' here")]
    public InputActionProperty menuButtonAction;

    // Timer to prevent accidental double-clicks
    private float cooldown = 0f;

    void OnTriggerStay(Collider other)
    {
        // 1. Check if the Menu Button is pressed (Value > 0.5)
        float buttonValue = menuButtonAction.action.ReadValue<float>();

        if (buttonValue > 0.5f && Time.time > cooldown)
        {
            // 2. Find the SceneLoader (Your manager that knows how to change scenes)
            SceneLoader loader = FindFirstObjectByType<SceneLoader>();
            
            if (loader == null) 
            {
                Debug.LogError("HandMenuClicker: Could not find SceneLoader in the scene!");
                return;
            }

            // 3. Check which button we are touching based on Tag
            if (other.CompareTag("HeavenButton"))
            {
                Debug.Log("Touched Heaven Button + Clicked Menu!");
                loader.LoadHeavenMode();
                cooldown = Time.time + 1.0f; // Wait 1 second before next click
            }
            else if (other.CompareTag("HellButton"))
            {
                Debug.Log("Touched Hell Button + Clicked Menu!");
                loader.LoadHellMode();
                cooldown = Time.time + 1.0f;
            }
            else if (other.CompareTag("ReturnButton"))
            {
                Debug.Log("Touched Return Button + Clicked Menu!");
                loader.LoadMenu();
                cooldown = Time.time + 1.0f;
            }
        }
    }
}