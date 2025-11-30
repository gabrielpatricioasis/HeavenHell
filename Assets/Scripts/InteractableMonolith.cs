using UnityEngine;

public class InteractableMonolith : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material materialHeaven;
    public Material materialHell;

    [Header("Audio Settings")] // NEW: Audio Clips from your Design Doc
    public AudioClip heavenClip;
    public AudioClip hellClip;

    [Header("Behavior Settings")]
    public float growthScaleFactor = 1.2f;
    public float shrinkScaleFactor = 0.8f;

    private Renderer myRenderer;
    private AudioSource audioSource; // NEW: To play the sound
    private Vector3 originalScale;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;

        // --- NEW: SETUP AUDIO ---
        audioSource = GetComponent<AudioSource>();
        // If the prefab doesn't have an AudioSource, add one automatically
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f; // Make it 3D sound
        }

        // --- NEW: SET INITIAL LOOK ---
        // Check the WorldManager immediately to see if we loaded into Heaven or Hell
        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
        {
            if (myRenderer != null) myRenderer.material = materialHeaven;
        }
        else
        {
            if (myRenderer != null) myRenderer.material = materialHell;
        }
    }

    public void ReceivePush()
    {
        // Check the state again (in case it changed)
        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
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
        
        // --- NEW: PLAY SOUND ---
        if (heavenClip != null) audioSource.PlayOneShot(heavenClip);
        
        Debug.Log("Monolith Growing (Heaven)");
    }

    void Shatter()
    {
        transform.localScale *= shrinkScaleFactor;
        myRenderer.material = materialHell;

        // --- NEW: PLAY SOUND ---
        if (hellClip != null) audioSource.PlayOneShot(hellClip);

        Debug.Log("Monolith Shattering (Hell)");
    }
    
    // Optional Polish: Slowly return to original size so you can push it again
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 0.5f);
    }
}