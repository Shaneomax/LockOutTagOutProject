
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnLook += HandleLook;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnLook -= HandleLook;
    }

    private void HandleLook(Vector2 lookInput)
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}