using UnityEngine;
using UnityEngine.InputSystem; // Needed for the button input

public class HandMenuClicker : MonoBehaviour
{
    [Header("Input Settings")]
    [Tooltip("We will assign the Menu button here")]
    public InputActionProperty menuButtonAction;

    [Header("Target")]
    public string targetTag = "ReturnButton";

    // Timer to prevent double-clicks
    private float cooldown = 0f;

    void OnTriggerStay(Collider other)
    {
        // 1. Are we touching the Return Button?
        if (other.CompareTag(targetTag))
        {
            // 2. Is the Menu Button pressed?
            // .ReadValue returns 1.0 if pressed, 0.0 if not.
            float buttonValue = menuButtonAction.action.ReadValue<float>();

            if (buttonValue > 0.5f && Time.time > cooldown)
            {
                Debug.Log("Menu Button Clicked on Return!");
                
                // 3. Find SceneLoader and go back
                SceneLoader loader = FindFirstObjectByType<SceneLoader>();
                if (loader != null)
                {
                    loader.LoadMenu();
                    cooldown = Time.time + 1.0f; // Wait 1 sec before clicking again
                }
            }
        }
    }
}