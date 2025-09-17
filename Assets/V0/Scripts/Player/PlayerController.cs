using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    public Camera playerCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool canMove = true;

    [Header("Interaction Settings")]
    public float interactCooldown = 0.2f;
    private bool canInteract = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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

    private void FixedUpdate()
    {
        if (!canMove) return;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleMove(Vector2 input)
    {
        moveInput = input;
    }

    public void HandleInteract()
    {
        if (!canInteract) return; 

        StartCoroutine(InteractWithCooldown());
    }

    private IEnumerator InteractWithCooldown()
    {
        canInteract = false;

        float interactDistance = 10f;
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

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

        yield return new WaitForSeconds(interactCooldown); 
        canInteract = true;
    }
}
