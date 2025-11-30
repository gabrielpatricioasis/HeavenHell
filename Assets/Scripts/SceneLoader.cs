using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 1. Keeps track of the choice (True = Heaven, False = Hell)
    public static bool isHeavenSelected = true;

    // 2. The exact name of your game scene
    // MAKE SURE THIS MATCHES YOUR SCENE FILE EXACTLY!
    public string gameSceneName = "Abstract_Garden"; 

    // Link this to the HEAVEN button
    public void LoadHeavenMode()
    {
        isHeavenSelected = true;
        SceneManager.LoadScene(gameSceneName);
    }

    // Link this to the HELL button
    public void LoadHellMode()
    {
        isHeavenSelected = false;
        SceneManager.LoadScene(gameSceneName);
    }
}