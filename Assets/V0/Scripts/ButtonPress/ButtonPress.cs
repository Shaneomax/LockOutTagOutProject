using UnityEngine;
using UnityEngine.Events;

public class ButtonPress : StepInteractable
{
    public UnityEvent onInteract;

    protected override void OnStepInteract()
    {
        if (StepManager.Instance.IsCurrentStep(this))
        {
            onInteract.Invoke();
            StepManager.Instance.CompleteCurrentStep();
        }
        
    }
}