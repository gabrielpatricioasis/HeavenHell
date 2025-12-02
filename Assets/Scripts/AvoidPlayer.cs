using UnityEngine;

public class AvoidPlayer : MonoBehaviour
{
    [Header("Settings")]
    public float baseDetectionRadius = 4.0f; // Distancia base
    public float moveSpeed = 15.0f;          // Velocidad de huida (RÁPIDA)

    private Transform playerTransform;

    void Start()
    {
        // Busca al Player automáticamente
        GameObject playerObj = GameObject.Find("XR Origin (VR)");
        if (playerObj != null) playerTransform = playerObj.transform;
        else
        {
            GameObject tagPlayer = GameObject.FindGameObjectWithTag("Player");
            if (tagPlayer != null) playerTransform = tagPlayer.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // --- TRUCO MATEMÁTICO ---
        // El radio de detección se multiplica por el tamaño actual del objeto.
        // Si el objeto es gigante (x3), el radio de detección será x3 (12 metros).
        float currentRadius = baseDetectionRadius * transform.localScale.x;

        if (distance < currentRadius)
        {
            // Huir en dirección opuesta
            Vector3 directionAway = transform.position - playerTransform.position;
            directionAway.y = 0;
            directionAway.Normalize();

            transform.position += directionAway * moveSpeed * Time.deltaTime;
        }
    }
}