using UnityEngine;
using UnityEngine.Events;

public class ButtonPress : StepInteractable
{
    public UnityEvent onInteract;

    protected override void OnStepInteract()
    {
        
            onInteract?.Invoke();

    }
}