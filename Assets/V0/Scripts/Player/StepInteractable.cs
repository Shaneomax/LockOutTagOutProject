using UnityEngine;

public abstract class StepInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (StepManager.Instance != null)
        {
            if (StepManager.Instance.IsCurrentStep(this))
            {
                OnStepInteract();
            }
        }
        else
        {
            OnStepInteract();
        }

        if (ZoomTrigger.ActiveZoomTrigger != null)
        {
            ZoomTrigger.ActiveZoomTrigger.ZoomOut();
        }
    }

    protected abstract void OnStepInteract();
}