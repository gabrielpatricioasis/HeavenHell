using UnityEngine;

public class SimpleMaterialSwap : MonoBehaviour
{
    [Header("Materials")]
    public Material materialHeaven; // Drag your Green/Blue ground here
    public Material materialHell;   // Drag your Red/Dark ground here

    void Start()
    {
        // 1. Get the Renderer
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        // 2. Ask WorldManager which world we are in
        // (Make sure your WorldManager script is in the scene!)
        if (WorldManager.Instance != null)
        {
            if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
            {
                rend.material = materialHeaven;
            }
            else
            {
                rend.material = materialHell;
            }
        }
    }
}