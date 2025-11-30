using UnityEngine;
using UnityEngine.SceneManagement; // <-- ESSENTIAL for scene management

public class SceneLoader : MonoBehaviour
{
    // Public method to load a scene by its string name
    public void LoadSceneByName(string sceneName)
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty! Cannot load scene.");
        }
    }
}