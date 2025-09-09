
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;

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
        float interactDistance = 10f;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // Debug ray in Scene view
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                Debug.Log($"Interacted with {hit.collider.name}");
            }
            else
            {
                Debug.Log("Hit object is not interactable.");
            }
        }
        else
        {
            Debug.Log("No object in range to interact.");
        }
    }
}
