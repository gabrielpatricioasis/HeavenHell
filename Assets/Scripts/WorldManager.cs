using UnityEngine;

// An "enum" is a clean way to define a list of possible states.
public enum WorldState { Heaven, Hell }

public class WorldManager : MonoBehaviour
{
    // This is our main state variable. We make it "public static" so that any other script
    // in the project can easily ask, "Hey, what state are we in?" without needing a direct link.
    public static WorldState currentState;

    // --- ASSETS TO SWAP ---
    // In the Unity Editor, we will drag the different assets for each state into these slots.
    public Material skyboxHeaven;
    public Material skyboxHell;
    // We could also add references here for Post-Processing profiles, music tracks, etc.


    void Start()
    {
        // When the experience begins, let's start in a neutral or heavenly state.
        SetState(WorldState.Heaven);
    }

    void Update()
    {
        // This is a simple test "switch" to change the state for debugging purposes.
        // When we press the Spacebar...
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ...we flip to the other state.
            if (currentState == WorldState.Heaven)
            {
                SetState(WorldState.Hell);
            }
            else
            {
                SetState(WorldState.Heaven);
            }
        }
    }

    // This is the core function that changes everything in the world.
    void SetState(WorldState newState)
    {
        currentState = newState;

        if (currentState == WorldState.Heaven)
        {
            // --- HEAVEN STATE LOGIC ---
            // Change the skybox to the "Heaven" version.
            RenderSettings.skybox = skyboxHeaven;
            // Later, we would also call functions here to change the music, lighting, post-processing, etc.
            Debug.Log("World state changed to: HEAVEN");
        }
        else // If the new state is Hell
        {
            // --- HELL STATE LOGIC ---
            // Change the skybox to the "Hell" version.
            RenderSettings.skybox = skyboxHell;
            // Later, we would add the logic for Hell's music, post-processing, etc.
            Debug.Log("World state changed to: HELL");
        }
    }
}