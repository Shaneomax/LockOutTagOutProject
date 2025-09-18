using UnityEngine;

public abstract class StepInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (StepManager.Instance != null)
        {
          
                OnStepInteract();

                if (ZoomTrigger.ActiveZoomTrigger != null)
                {
                    ZoomTrigger.ActiveZoomTrigger.RegisterTaskCompletion(this);
                }
           
        }
        else
        {
            OnStepInteract();

            if (ZoomTrigger.ActiveZoomTrigger != null)
            {
                ZoomTrigger.ActiveZoomTrigger.RegisterTaskCompletion(this);
            }
        }
    }

    protected abstract void OnStepInteract();
}