using UnityEngine;
// We no longer need the InputSystem for this script.

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject returnButtonObject; 

    void Start() // Changed from Awake to Start
    {
        // Make sure the button starts off invisible.
        if (returnButtonObject != null)
        {
            returnButtonObject.SetActive(false);
        }
    }

    void Update()
    {
        // --- THIS IS THE FIX ---
        // Every frame, check if the "M" key on the keyboard was pressed.
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (returnButtonObject != null)
            {
                // Toggle the button's visibility.
                bool isCurrentlyVisible = returnButtonObject.activeSelf;
                returnButtonObject.SetActive(!isCurrentlyVisible);
            }
        }
    }
}