using UnityEngine;

public abstract class StepInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (StepManager.Instance.IsCurrentStep(this))
        {
            OnStepInteract();
        }
        
    }

    protected abstract void OnStepInteract();
}