using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SimpleReturn : MonoBehaviour
{
    [Header("Nombre de la Escena del Menú")]
    public string menuSceneName = "Start Screen"; // <--- PON AQUÍ EL NOMBRE EXACTO

    [Header("Botón VR (Arrastra Referencia)")]
    // Usaremos el botón de Menú del mando izquierdo (las rayitas)
    public InputActionReference menuButton;

    void Update()
    {
        // OPCIÓN 1: TECLADO (Para probar ya)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            GoHome();
        }

        // OPCIÓN 2: VR (Botón Menú)
        if (menuButton != null && menuButton.action.WasPerformedThisFrame())
        {
            GoHome();
        }
    }

    void GoHome()
    {
        Debug.Log("Volviendo a casa...");
        SceneManager.LoadScene(menuSceneName);
    }
}