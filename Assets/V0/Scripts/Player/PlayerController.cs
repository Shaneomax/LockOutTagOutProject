
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    public Camera playerCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private void Start()
    {
            InputManager.Instance.OnMove += HandleMove;
            InputManager.Instance.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
            InputManager.Instance.OnMove -= HandleMove;
            InputManager.Instance.OnInteract -= HandleInteract;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleMove(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleInteract()
    {
        float interactDistance = 5f;
        Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());

        // Debug ray in Scene view
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                Debug.Log($"Interacted with {hit.collider.name} (component: {interactable.GetType().Name})");
            }
            else
            {
                Debug.Log("Hit object is not interactable.");
            }
        }

    }
}
