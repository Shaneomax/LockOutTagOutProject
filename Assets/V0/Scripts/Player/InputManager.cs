using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnInteract;

    public bool CanLook { get; set; } = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        if (context.performed || context.canceled)
            OnMove?.Invoke(move);
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (!CanLook) return; 

        Vector2 look = context.ReadValue<Vector2>();
        OnLook?.Invoke(look);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteract?.Invoke();
    }
}