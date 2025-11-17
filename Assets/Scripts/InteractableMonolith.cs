using UnityEngine;

public class InteractableMonolith : MonoBehaviour
{
    // --- ASSETS TO SWAP ---
    // In the Unity Editor (on the prefab), we'll drag the materials for each state.
    public Material materialHeaven;
    public Material materialHell;

    [Header("Behavior Settings")]
    public float growthScaleFactor = 1.2f; // How much it grows each time in Heaven.
    public float shrinkScaleFactor = 0.8f; // How much it shrinks each time in Hell.

    private Renderer myRenderer;
    private Vector3 originalScale;

    void Start()
    {
        // It's efficient to get and store these components at the start.
        myRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;
    }

    // This is the main public function that the PLAYER's interaction script will call.
    // This script doesn't need to know *how* it was pushed, only that it *was* pushed.
    public void ReceivePush()
    {
        // This script asks the WorldManager what the current state is.
        if (WorldManager.currentState == WorldState.Heaven)
        {
            Grow();
        }
        else // If the state is Hell
        {
            Shatter();
        }
    }

    void Grow()
    {
        // Make the object bigger and apply the Heaven material.
        transform.localScale *= growthScaleFactor;
        myRenderer.material = materialHeaven;
        Debug.Log("Monolith is Growing.");
        // We would trigger the "growth" sound effect and the swelling haptic feedback here.
    }

    void Shatter()
    {
        // Make the object smaller and apply the Hell material.
        transform.localScale *= shrinkScaleFactor;
        myRenderer.material = materialHell;
        Debug.Log("Monolith is Shattering.");
        // We would trigger the "shatter" sound effect and the jarring haptic feedback here.
    }
}
