using UnityEngine;

public class InteractableEnergyStream : MonoBehaviour
{
    [Header("Heaven State Settings")]
    public Color colorHeaven = Color.yellow;
    public float intensityHeaven = 200f; // More particles per second.

    [Header("Hell State Settings")]
    public Color colorHell = Color.red;
    public float intensityHell = 50f;   // Fewer, more chaotic particles.

    private ParticleSystem myParticleSystem;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    // Store the original values so we can return to a neutral state if needed.
    private Color originalColor;
    private float originalIntensity;

    void Start()
    {
        // Get and store the particle system modules for efficiency.
        myParticleSystem = GetComponent<ParticleSystem>();
        mainModule = myParticleSystem.main;
        emissionModule = myParticleSystem.emission;

        // Save the initial state of the particle system.
        originalColor = mainModule.startColor.color;
        originalIntensity = emissionModule.rateOverTime.constant;
    }

    // The player's script calls this function when the trigger hold BEGINS.
    public void StartTriggerHold()
    {
        if (WorldManager.currentState == WorldState.Heaven)
        {
            Harmonize();
        }
        else // If the state is Hell
        {
            Drain();
        }
    }

    // The player's script calls this function when the trigger hold ENDS.
    public void StopTriggerHold()
    {
        // For now, let's make the stream return to its neutral state when the user lets go.
        // (Alternatively, we could decide to make the change permanent).
        mainModule.startColor = originalColor;
        emissionModule.rateOverTime = originalIntensity;
        Debug.Log("Energy Stream returning to neutral.");
    }

    void Harmonize()
    {
        // Change the color and increase the flow rate.
        mainModule.startColor = colorHeaven;
        emissionModule.rateOverTime = intensityHeaven;
        Debug.Log("Energy Stream is Harmonizing.");
        // We would trigger the "harmonious" sound and the soft haptic pulse here.
    }

    void Drain()
    {
        // Change the color and decrease the flow rate.
        mainModule.startColor = colorHell;
        emissionModule.rateOverTime = intensityHell;
        Debug.Log("Energy Stream is Draining.");
        // We would trigger the "draining" sound and the grating haptic feedback here.
    }
}
