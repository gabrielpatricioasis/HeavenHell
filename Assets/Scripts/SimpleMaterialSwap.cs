using UnityEngine;

public class SimpleMaterialSwap : MonoBehaviour
{
    [Header("Materials")]
    public Material materialHeaven;
    public Material materialHell;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        // Ejecutar una vez al inicio
        CheckAndSwap();
    }

    void Update()
    {
        // Ejecutar constantemente para detectar el cambio con la tecla Enter
        CheckAndSwap();
    }

    void CheckAndSwap()
    {
        // Seguridad: Si no hay renderer o WorldManager, no hacemos nada
        if (rend == null || WorldManager.Instance == null) return;

        if (WorldManager.Instance.currentState == WorldManager.WorldState.Heaven)
        {
            // Solo cambiamos el material si no es el correcto (para no gastar recursos)
            if (rend.sharedMaterial != materialHeaven)
            {
                rend.material = materialHeaven;
            }
        }
        else // Hell
        {
            if (rend.sharedMaterial != materialHell)
            {
                rend.material = materialHell;
            }
        }
    }
}