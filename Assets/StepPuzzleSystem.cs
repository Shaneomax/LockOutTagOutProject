using UnityEngine;
using UnityEngine.InputSystem;

public class StepPuzzleSystem : MonoBehaviour
{
    [Header("Lights")]
    public Light redLight;
    public Light yellowLight;

    [Header("Keys")]
    public Transform key1;
    public Transform key2;

    [Header("Camera")]
    public Camera playerCamera;
    public float rayDistance = 5f;

    private bool keysSwapped = false;
    private bool yellowButtonPressed = false;

    void Start()
    {
        yellowLight.enabled = false;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        bool redLightActive = redLight.enabled;

        Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Key") && redLightActive && !keysSwapped)
            {
                SwapKeys();
            }

            if (hit.collider.CompareTag("YellowButton") && redLightActive && keysSwapped && !yellowButtonPressed)
            {
                yellowButtonPressed = true;
                yellowLight.enabled = true;
                Debug.Log("Yellow light activated!");

                CheckDoorUnlock();
            }
        }
    }

    void SwapKeys()
    {
        Vector3 pos1 = key1.position;
        Vector3 pos2 = key2.position;

        key1.position = new Vector3(pos2.x, pos1.y, pos1.z);
        key2.position = new Vector3(pos1.x, pos2.y, pos2.z);

        keysSwapped = true;
        Debug.Log("Keys swapped!");
    }

    void CheckDoorUnlock()
    {
        if (redLight.enabled && keysSwapped && yellowButtonPressed)
        {
            Debug.Log("Door unlocked!");
        }
    }
}
