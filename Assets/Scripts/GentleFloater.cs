using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GentleFloater : MonoBehaviour
{
    public float moveSpeed = 1.5f;

    // --- THE FIX ---
    // We set huge default numbers here (-1000 to 1000).
    // This means "If nobody tells me where the walls are, assume the room is huge."
    // This prevents them from snapping to 0,0.
    public Vector2 xLimits = new Vector2(-1000, 1000); 
    public Vector2 zLimits = new Vector2(-1000, 1000); 

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        
        // Note: Using older API 'drag'/AngularDrag' to be safe, 
        // Unity 6 will auto-upgrade or accept it.
        rb.linearDamping = 0f; 
        rb.angularDamping = 0f;

        rb.constraints = RigidbodyConstraints.FreezePositionY | 
                         RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationZ;
        
        PushObject(rb);
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Keep moving
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            PushObject(rb);
        }

        CheckBoundaries(rb);
    }

    void CheckBoundaries(Rigidbody rb)
    {
        // SAFETY CHECK: If limits are somehow zero (broken), don't do anything.
        if (xLimits == Vector2.zero && zLimits == Vector2.zero) return;

        Vector3 pos = transform.position;
        Vector3 vel = rb.linearVelocity;
        bool hitWall = false;

        // Check X
        if (pos.x > xLimits.y) 
        {
            pos.x = xLimits.y;
            vel.x = -Mathf.Abs(vel.x); 
            hitWall = true;
        }
        else if (pos.x < xLimits.x) 
        {
            pos.x = xLimits.x; 
            vel.x = Mathf.Abs(vel.x);
            hitWall = true;
        }

        // Check Z
        if (pos.z > zLimits.y) 
        {
            pos.z = zLimits.y;
            vel.z = -Mathf.Abs(vel.z);
            hitWall = true;
        }
        else if (pos.z < zLimits.x) 
        {
            pos.z = zLimits.x;
            vel.z = Mathf.Abs(vel.z);
            hitWall = true;
        }

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