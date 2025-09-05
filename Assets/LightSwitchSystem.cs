using UnityEngine;
using UnityEngine.InputSystem;

public class LightSwitchSystem : MonoBehaviour
{
    public Light greenLight1;
    public Light greenLight2;
    public Light redLight;

    public Camera playerCamera; 
    public float rayDistance = 5f;

    void Start()
    {
       
        greenLight1.enabled = true;
        greenLight2.enabled = true;
        redLight.enabled = false;
    }


    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag("Switch"))
                {
                    greenLight1.enabled = false;
                    greenLight2.enabled = false;
                    redLight.enabled = true;
                }
            }
        }
    }
}