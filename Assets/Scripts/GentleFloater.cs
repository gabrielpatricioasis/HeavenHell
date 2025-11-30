using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GentleFloater : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;

    [Header("Repulsion Settings (VR Interaction)")]
    public float detectRadius = 5.0f;  // How close player must be to push it
    public float pushStrength = 500.0f; // How hard it pushes away

    [Header("Boundaries (Set by Spawner)")]
    // Default huge limits so they don't squeeze if spawner is slow
    public Vector2 xLimits = new Vector2(-1000, 1000);
    public Vector2 zLimits = new Vector2(-1000, 1000);

    private Transform playerHead; // Reference to your VR Headset

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        
        // 1. Setup Physics
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearDamping = 1.0f; // Higher drag helps the push feel smoother
        rb.angularDamping = 0.5f;

        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 2. Find the VR Camera (The Player)
        if (Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("GentleFloater could not find a Main Camera tagged object!");
        }
        
        // 3. Initial Push
        PushObject(rb);
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // A. Maintain minimal movement (The Drift)
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            PushObject(rb);
        }

        // B. CHECK WALLS
        CheckBoundaries(rb);

        // C. CHECK PLAYER (The Repulsion)
        ApplyPlayerRepulsion(rb);
    }

    void ApplyPlayerRepulsion(Rigidbody rb)
    {
        if (playerHead == null) return;

        // 1. Calculate distance between Object and Player Head
        float distanceToPlayer = Vector3.Distance(transform.position, playerHead.position);

        // 2. Are we inside the danger zone?
        if (distanceToPlayer < detectRadius)
        {
            // Calculate direction FROM Player TO Object
            Vector3 pushDir = transform.position - playerHead.position;
            
            // IMPORTANT: Flatten the Y so they don't fly up/down
            pushDir.y = 0; 
            pushDir.Normalize();

            // Calculate force: The closer you are, the stronger the push
            // We use (1 / distance) to make it exponential
            float dynamicForce = pushStrength / distanceToPlayer;

            // Apply the force to the Rigidbody
            rb.AddForce(pushDir * dynamicForce, ForceMode.Force);
        }
    }

    void CheckBoundaries(Rigidbody rb)
    {
        if (xLimits == Vector2.zero && zLimits == Vector2.zero) return;

        Vector3 pos = transform.position;
        Vector3 vel = rb.linearVelocity;
        bool hitWall = false;

        if (pos.x > xLimits.y) { pos.x = xLimits.y; vel.x = -Mathf.Abs(vel.x); hitWall = true; }
        else if (pos.x < xLimits.x) { pos.x = xLimits.x; vel.x = Mathf.Abs(vel.x); hitWall = true; }

        if (pos.z > zLimits.y) { pos.z = zLimits.y; vel.z = -Mathf.Abs(vel.z); hitWall = true; }
        else if (pos.z < zLimits.x) { pos.z = zLimits.x; vel.z = Mathf.Abs(vel.z); hitWall = true; }

        if (hitWall)
        {
            transform.position = pos;
            rb.linearVelocity = vel;
        }
    }

    void PushObject(Rigidbody rb)
    {
        if (rb.IsSleeping()) rb.WakeUp();
        Vector3 flatDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        rb.linearVelocity = flatDirection * moveSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 bounceDir = (transform.position - collision.transform.position).normalized;
        bounceDir.y = 0; 
        rb.linearVelocity = bounceDir * moveSpeed;
    }
}