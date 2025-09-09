using UnityEngine;
using System.Collections;

public class Keys : MonoBehaviour,IInteractable
{
    [Header("Movement Settings")]
    public Vector3 targetPosition; 
    public float speed = 3f;


    public void MoveAndHide()
    {
        StartCoroutine(MoveToTargetAndDeactivate());
    }

    private IEnumerator MoveToTargetAndDeactivate()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        gameObject.SetActive(false); 
    }

    public void Interact()
    {
        MoveAndHide();
    }
}