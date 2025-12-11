using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    public GameObject returnButtonObject; // El cartel de Créditos/Controles (Opcional)

    void Start()
    {
        // Asegurarnos de que el ratón se ve en el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (returnButtonObject != null)
        {
            returnButtonObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Mostrar/Ocultar Créditos con 'M'
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (returnButtonObject != null)
            {
                bool isCurrentlyVisible = returnButtonObject.activeSelf;
                returnButtonObject.SetActive(!isCurrentlyVisible);
            }
        }

        // 2. SALIR DEL JUEGO CON ESCAPE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("¡SALIENDO DEL JUEGO!");

            // Cierra el juego si es una Build (.exe)
            Application.Quit();

            // Cierra el Play Mode si estás en el Editor de Unity (TRUCO PRO)
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}