using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SlidingDoor : StepInteractable
{
    public List<Transform> doorParts = new List<Transform>();
    public float moveSpeed = 1f; 

    private bool isOpening = false;

    protected override void OnStepInteract()
    {
        if (!isOpening)
            OpenDoorSequence();
    }

    private void OpenDoorSequence()
    {
        if (doorParts.Count < 2) return;

        isOpening = true;

        Sequence seq = DOTween.Sequence();

        for (int i = 1; i < doorParts.Count; i++)
        {
            int index = i;
            seq.AppendCallback(() =>
            {
                // Move all previous door parts to align with current part
                Sequence innerSeq = DOTween.Sequence();
                for (int j = 0; j < index; j++)
                {
                    Vector3 target = new Vector3(
                        doorParts[index].position.x,
                        doorParts[j].position.y,
                        doorParts[index].position.z
                    );

                    float distance = Vector3.Distance(doorParts[j].position, target);
                    float duration = distance / moveSpeed;

                    innerSeq.Join(doorParts[j].DOMove(target, duration));
                }
                innerSeq.Play();
            });

            // Wait until inner sequence finishes
            float maxDistance = 0f;
            for (int j = 0; j < i; j++)
                maxDistance = Mathf.Max(maxDistance, Vector3.Distance(doorParts[j].position, doorParts[i].position));
            float waitTime = maxDistance / moveSpeed;

            seq.AppendInterval(waitTime);
        }

        seq.OnComplete(() =>
        {
            isOpening = false;
            StepManager.Instance.CompleteCurrentStep();
        });

        seq.Play();
    }
}
