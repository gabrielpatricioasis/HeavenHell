using UnityEngine;

public class MouseClicker : MonoBehaviour
{
    private SceneLoader sceneLoader;

    void Start()
    {
        // 1. Buscamos el cargador de escenas
        sceneLoader = FindFirstObjectByType<SceneLoader>();

        // 2. IMPORTANTE: Desbloquear el ratón para poder usarlo en el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Si hacemos Clic Izquierdo (0)
        if (Input.GetMouseButtonDown(0))
        {
            // Lanzamos un rayo desde la cámara hacia donde está el ratón
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                // Verificamos qué hemos tocado
                CheckButton(hit.collider);
            }
        }
    }

    void CheckButton(Collider other)
    {
        if (sceneLoader == null) return;

        if (other.CompareTag("HeavenButton"))
        {
            Debug.Log("PC: Click en Heaven");
            sceneLoader.LoadHeavenMode();
        }
        else if (other.CompareTag("HellButton"))
        {
            Debug.Log("PC: Click en Hell");
            sceneLoader.LoadHellMode();
        }
        // Si tienes un botón de Salir o Return en el menú principal
        else if (other.CompareTag("ReturnButton"))
        {
            Debug.Log("PC: Click en Return");
            // Aquí puedes llamar a lo que haga tu botón de return, 
            // o desactivar el objeto si es un popup.
            other.gameObject.SetActive(false);
        }
    }
}