using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public float MoveSpeed = 5f;

    private Vector2 _moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            _moveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            _moveInput = Vector2.zero;
    }

    private void Update()
    {

        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);
        transform.Translate(move * MoveSpeed * Time.deltaTime);
    }

}