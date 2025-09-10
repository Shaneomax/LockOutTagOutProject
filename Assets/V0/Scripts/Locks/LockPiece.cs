using UnityEngine;

public class LockPiece : MonoBehaviour, IInteractable
{
    public LockSystem system;
    public Vector3 targetLocalEuler = new Vector3(0, 90, 0);
    public GameObject cardObject;

    public bool solved = false;

    public void Interact()
    {
        system.TryUnlock(this);
    }
}