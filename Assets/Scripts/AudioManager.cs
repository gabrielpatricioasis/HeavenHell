using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Referencias")]
    public AudioSource ambientSource;

    [Header("Audio Clips")]
    public AudioClip heavenAmbient;
    public AudioClip hellAmbient;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (ambientSource == null) ambientSource = GetComponent<AudioSource>();
    }

    public void ChangeAmbient(WorldManager.WorldState newState)
    {
        AudioClip targetClip = null;

        if (newState == WorldManager.WorldState.Heaven)
        {
            targetClip = heavenAmbient;
        }
        else
        {
            targetClip = hellAmbient;
        }

        if (ambientSource.clip != targetClip)
        {
            ambientSource.clip = targetClip;
            ambientSource.Play();
        }
    }
}