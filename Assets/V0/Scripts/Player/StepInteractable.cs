using UnityEngine;

public abstract class StepInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (StepManager.Instance.IsCurrentStep(this))
        {
            OnStepInteract();
        }
        else
        {
            Debug.Log($"Cannot interact with {name}, not current step.");
        }
    }

    protected abstract void OnStepInteract();
}