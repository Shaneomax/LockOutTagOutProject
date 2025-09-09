
using UnityEngine;
using UnityEngine.Events;

public class ButtonPress : MonoBehaviour, IInteractable
{
    [Header("Events")]
    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract.Invoke();
    }
}