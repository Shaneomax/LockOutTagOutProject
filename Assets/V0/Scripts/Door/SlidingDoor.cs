using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlidingDoor : StepInteractable
{
    public List<Transform> doorParts = new List<Transform>();
    public float moveSpeed = 2f;

    private bool isOpening = false;

    protected override void OnStepInteract()
    {
        if (!isOpening)
            StartCoroutine(DoorOpening());
    }

    private IEnumerator DoorOpening()
    {
        isOpening = true;

        for (int i = 1; i < doorParts.Count; i++)
        {
            bool moving = true;
            while (moving)
            {
                moving = false;
                for (int j = 0; j < i; j++)
                {
                    Vector3 target = new Vector3(
                        doorParts[i].position.x,
                        doorParts[j].position.y,
                        doorParts[i].position.z
                    );

                    doorParts[j].position = Vector3.MoveTowards(
                        doorParts[j].position,
                        target,
                        moveSpeed * Time.deltaTime
                    );

                    if ((doorParts[j].position - target).sqrMagnitude > 0.0001f)
                        moving = true;
                }
                yield return null;
            }
        }

        isOpening = false;

        StepManager.Instance.CompleteCurrentStep();
    }
}