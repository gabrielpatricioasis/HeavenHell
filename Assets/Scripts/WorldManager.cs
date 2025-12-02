using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // --- FIX: ENUM MOVED INSIDE THE CLASS ---
    public enum WorldState { Heaven, Hell }
    // ----------------------------------------

    // Singleton Instance
    public static WorldManager Instance;

    // Your main state variable
    // Note: I removed 'static' here so it works better with the Inspector, 
    // but kept 'Instance' static so you can access it.
    public WorldState currentState;

    [Header("Assets to Swap")]
    public Material skyboxHeaven;
    public Material skyboxHell;

    [Header("Physics Settings")]
    public float heavenGravity = -4.0f; 
    public float hellGravity = -20.0f;  

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Read choice from SceneLoader
        if (SceneLoader.isHeavenSelected)
        {
            SetState(WorldState.Heaven);
        }
        else
        {
            SetState(WorldState.Hell);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentState == WorldState.Heaven) SetState(WorldState.Hell);
            else SetState(WorldState.Heaven);
        }
    }

    public void SetState(WorldState newState)
    {
        currentState = newState;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeAmbient(newState);
        }

        if (currentState == WorldState.Heaven)
        {
            if (skyboxHeaven != null) RenderSettings.skybox = skyboxHeaven;
            Physics.gravity = new Vector3(0, heavenGravity, 0);
            Debug.Log("World Set to: HEAVEN");
        }
        else 
        {
            if (skyboxHell != null) RenderSettings.skybox = skyboxHell;
            Physics.gravity = new Vector3(0, hellGravity, 0);
            Debug.Log("World Set to: HELL");
        }
    }
}