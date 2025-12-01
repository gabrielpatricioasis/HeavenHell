using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static bool isHeavenSelected = true;
    public string gameSceneName = "Abstract_Garden";
    
    // --- NEW: Name of your menu scene ---
    public string menuSceneName = "Start Screen"; 

    public void LoadHeavenMode()
    {
        isHeavenSelected = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadHellMode()
    {
        isHeavenSelected = false;
        SceneManager.LoadScene(gameSceneName);
    }

    // --- NEW: Function to go back ---
    public void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}