using UnityEngine;
using System.Collections;

public class Keys : StepInteractable
{
    public Vector3 targetPosition;
    public float speed = 3f;

    protected override void OnStepInteract()
    {
        StartCoroutine(MoveAndComplete());
    }

    private IEnumerator MoveAndComplete()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        gameObject.SetActive(false);

        StepManager.Instance.CompleteCurrentStep();
    }
}