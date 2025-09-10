using UnityEngine;

public class LockPiece : StepInteractable
{
    public LockSystem system;
    public Vector3 targetLocalEuler = Vector3.zero;
    public GameObject cardObject;

    public bool solved = false;

    protected override void OnStepInteract()
    {
        system.TryUnlock(this);
        StepManager.Instance.CompleteCurrentStep();
    }
}