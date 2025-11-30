using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This is the most important part of the "Singleton" pattern.
    // It creates a "public static" variable, meaning any other script can access it
    // easily by simply typing "GameManager.Instance".
    public static GameManager Instance;

    // An "enum" is a custom variable type that can only be one of the values in the list.
    // This prevents typos and makes the code cleaner than using simple strings.
    public enum PlayerChoice
    {
        None,
        Heaven,
        Hell
    }

    // This public variable will hold the choice the player makes.
    // The SceneLoader script in your StartScene will set this value.
    public PlayerChoice choice;

    // The Awake function is called by Unity before any Start() functions,
    // which is perfect for setting up a manager like this.
    private void Awake()
    {
        // This logic ensures that there is only ever ONE GameManager in the entire game.
        if (Instance == null)
        {
            // If this is the very first time a GameManager has been created,
            // it assigns itself to the static "Instance".
            Instance = this;

            // This is the most important line in the script.
            // It tells Unity: "Do not destroy the GameObject this script is attached to
            // when you load a new scene." This is how it persists.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another GameManager instance already exists (which can happen if you
            // reload the start scene), this new one immediately destroys itself to
            // prevent duplicates.
            Destroy(gameObject);
        }
    }
}