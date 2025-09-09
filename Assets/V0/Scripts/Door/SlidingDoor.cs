using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IInteractable
{
    public Transform[] doorParts;
    public float moveSpeed = 2f;

    private bool isOpening;

    public void Interact()
    {
        if (!isOpening)
            StartCoroutine(DoorOpening());
    }

    private IEnumerator DoorOpening()
    {
        for (int i = 1; i < doorParts.Length; i++)
        {
            bool done = true;
            while (done)
            {
                done = false;

                for (int j = 0; j < i; j++)
                {
                    Vector3 target = new Vector3(doorParts[i].position.x, doorParts[j].position.y, doorParts[i].position.z);
                    doorParts[j].position = Vector3.MoveTowards(doorParts[j].position, target, moveSpeed * Time.deltaTime);
                    if (doorParts[j].position != target)
                        done = true;
                }

                yield return null;
            }
        }
    }
}