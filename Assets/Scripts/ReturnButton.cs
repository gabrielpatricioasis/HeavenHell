using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    // You can change this in the Inspector if your scene has a different name.
    public string startSceneName = "Start Screen"; 

    // This function will be called when the player "clicks" the button in VR.
    public void GoToStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }
}