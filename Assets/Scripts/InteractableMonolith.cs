using UnityEngine;

public class InteractableMonolith : MonoBehaviour
{
    // --- ASSETS TO SWAP ---
    public Material materialHeaven;
    public Material materialHell;

    [Header("Behavior Settings")]
    public float growthScaleFactor = 1.2f;
    
    // --- NEW: A slot for our custom shatter effect ---
    public GameObject shatterEffectPrefab;

    private Renderer myRenderer;
    private bool hasBeenInteractedWith = false;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        
        if (WorldManager.currentState == WorldState.Heaven)
        {
            myRenderer.material = materialHeaven;
        }
        else
        {
            myRenderer.material = materialHell;
        }
    }

    public void ReceivePush()
    {
        if (hasBeenInteractedWith) return;
        hasBeenInteractedWith = true;

        if (WorldManager.currentState == WorldState.Heaven)
        {
            Grow();
        }
        else
        {
            Shatter();
        }
    }

    void Grow()
    {
        transform.localScale *= growthScaleFactor;
        myRenderer.material = materialHeaven;
        Debug.Log("Monolith is Growing.");
    }

    // --- THIS IS THE NEW SHATTER FUNCTION ---
    void Shatter()
    {
        Debug.Log("Shattering with particle effect!");

        // If we have assigned a shatter effect prefab in the Inspector...
        if (shatterEffectPrefab != null)
        {
            // ...create an instance of the effect at the monolith's position.
            Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);
        }

        // Now, destroy the original monolith object completely.
        Destroy(gameObject);
    }
}