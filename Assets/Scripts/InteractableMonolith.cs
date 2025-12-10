using UnityEngine;

public class InteractableMonolith : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material materialHeaven;
    public Material materialHell;

    [Header("Audio Settings")]
    public AudioClip heavenClip;
    public AudioClip hellClip;

    [Header("Behavior Settings")]
    public float growthScaleFactor = 2.5f; // Tamaño en Heaven
    public float rubbleScaleFactor = 0.0f; // Tamaño en Hell
    public float animationSpeed = 5.0f;

    [Header("Effects")]
    public GameObject shatterEffectPrefab;

    private Renderer myRenderer;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;
        targetScale = originalScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }

        UpdateMaterial();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (myRenderer == null || WorldManager.Instance == null) return;

        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
        {
            if (myRenderer.sharedMaterial != materialHeaven) myRenderer.material = materialHeaven;
        }
        else
        {
            if (myRenderer.sharedMaterial != materialHell) myRenderer.material = materialHell;
        }
    }

    public void ReceivePush()
    {
        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
            Grow();
        else
            Shatter();
    }

    void Grow()
    {
        // Crecimiento normal, sin partículas
        targetScale = originalScale * growthScaleFactor;
        if (heavenClip != null) audioSource.PlayOneShot(heavenClip);
    }

    void Shatter()
    {
        // Destrucción (Implosión + Partículas)
        targetScale = originalScale * rubbleScaleFactor;

        if (hellClip != null) audioSource.PlayOneShot(hellClip);

        if (shatterEffectPrefab != null)
            Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);
    }
}