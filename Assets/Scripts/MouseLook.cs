using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    // Multiplicador extra solo para Mac (porque las pantallas Retina leen el input muy bajo)
    public float macMultiplier = 8.0f;

    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Calculamos la sensibilidad base
        float currentSens = mouseSensitivity;

        // 2. DETECTOR DE MAC: Si el juego corre en Mac, multiplicamos la sensibilidad
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            currentSens *= macMultiplier;
        }

        // 3. Aplicamos el movimiento con la sensibilidad ajustada
        float mouseX = Input.GetAxis("Mouse X") * currentSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * currentSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}