using UnityEngine;
using UnityEngine.UI; // Necesario para la UI

public class SimpleCrosshair : MonoBehaviour
{
    public Image crosshairImage; // Arrastra tu puntito de mira aquí
    public float reachDistance = 100f;

    // Colores de feedback
    public Color colorNormal = Color.green;
    public Color colorInteract = Color.red;    // Se pondrá ROJO si puedes disparar
    public Color colorMagic = Color.magenta;   // Se pondrá MAGENTA si es un Stream

    void Update()
    {
        if (crosshairImage == null) return;

        // Lanzamos un rayo invisible desde el centro
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            // ¿Es un Monolito?
            if (hit.collider.GetComponent<InteractableMonolith>())
            {
                crosshairImage.color = colorInteract; // ¡ROJO! DISPARA
            }
            // ¿Es un Energy Stream?
            else if (hit.collider.GetComponent<InteractableEnergyStream>())
            {
                crosshairImage.color = colorMagic;    // ¡MAGENTA! USA MAGIA
            }
            // ¿Es otra cosa (suelo, pared)?
            else
            {
                crosshairImage.color = colorNormal;   // Verde
            }
        }
        else
        {
            crosshairImage.color = colorNormal; // Verde
        }
    }
}