using UnityEngine;

public class GameChoiceManager : MonoBehaviour
{
    // Static field to hold the persistent choice (Heaven or Hell)
    public static string playerChoice = "";

    // Static reference to ensure only one instance of this manager exists
    private static GameChoiceManager instance;

    void Awake()
    {
        // Enforce the Singleton Pattern: Only one instance should exist.
        if (instance == null)
        {
            // If this is the first instance, make it the main instance
            instance = this;

            // This is the CRUCIAL line: It keeps this GameObject alive 
            // when LoadScene is called.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate.
            Destroy(gameObject);
        }
    }
}