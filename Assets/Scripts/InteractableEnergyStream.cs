using UnityEngine;

public class InteractableEnergyStream : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource streamAudioSource;

    [Header("Heaven Settings (Creator)")]
    public Color colorHeaven = new Color(1f, 0.92f, 0.016f);

    [Space(10)]
    public float rateHeavenIdle = 500f;
    public float sizeHeavenIdle = 1.0f;
    public float speedHeavenIdle = 5.0f;

    [Space(10)]
    public float rateHeavenInteract = 8000f;
    public float sizeHeavenInteract = 3.5f;
    public float speedHeavenInteract = 25.0f;
    public AudioClip soundHeaven;

    [Header("Hell Settings (Corruptor)")]
    public Color colorHell = Color.gray;

    [Space(10)]
    public float rateHellIdle = 500f;
    public float sizeHellIdle = 0.85f;
    public float speedHellIdle = 2.0f;

    [Space(10)]
    public float rateHellInteract = 0f;
    public float sizeHellInteract = 0f;
    public float speedHellInteract = 0f;
    public AudioClip soundHell;

    private ParticleSystem myParticleSystem;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private bool isInteracting = false;

    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        mainModule = myParticleSystem.main;
        emissionModule = myParticleSystem.emission;

        if (streamAudioSource == null) streamAudioSource = GetComponent<AudioSource>();
        if (WorldManager.Instance != null) UpdateVisuals();
    }

    void Update()
    {
        if (WorldManager.Instance != null) UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
        {
            mainModule.startColor = colorHeaven;
            if (isInteracting)
            {
                emissionModule.rateOverTime = rateHeavenInteract;
                mainModule.startSize = sizeHeavenInteract;
                mainModule.startSpeed = speedHeavenInteract;
            }
            else
            {
                emissionModule.rateOverTime = rateHeavenIdle;
                mainModule.startSize = sizeHeavenIdle;
                mainModule.startSpeed = speedHeavenIdle;
            }
        }
        else
        {
            mainModule.startColor = colorHell;
            if (isInteracting)
            {
                emissionModule.rateOverTime = rateHellInteract;
                mainModule.startSize = sizeHellInteract;
                mainModule.startSpeed = speedHellInteract;
            }
            else
            {
                emissionModule.rateOverTime = rateHellIdle;
                mainModule.startSize = sizeHellIdle;
                mainModule.startSpeed = speedHellIdle;
            }
        }
    }

    public void StartTriggerHold()
    {
        isInteracting = true;
        AudioClip clipToPlay = (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven) ? soundHeaven : soundHell;

        if (streamAudioSource != null && clipToPlay != null)
        {
            streamAudioSource.clip = clipToPlay;
            streamAudioSource.Play();
        }
    }

    public void StopTriggerHold()
    {
        isInteracting = false;
        if (streamAudioSource != null) streamAudioSource.Stop();
    }
}