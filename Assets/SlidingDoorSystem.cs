using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SlidingDoorSystem : MonoBehaviour
{
    [Header("Door Parts (order 1-4)")]
    public Transform part1;
    public Transform part2;
    public Transform part3;
    public Transform part4;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;      
    public float rayDistance = 5f;   

    [Header("Refs")]
    public Camera playerCamera;
    public StepPuzzleSystem puzzleSystem;

    private bool isOpening = false;

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed || isOpening) return;

        // Must be unlocked by puzzle
        if (puzzleSystem == null || !puzzleSystem.IsDoorUnlocked)
        {
            Debug.Log("Door is locked!");
            return;
        }

        // Must click the door
        Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance) && hit.collider.CompareTag("Door"))
        {
            StartCoroutine(OpenDoorSequence());
        }
    }

    private IEnumerator OpenDoorSequence()
    {
        isOpening = true;

        yield return MovePartToTarget(part1, part2.position);

        yield return MoveMultipleParts(new[] { part1, part2 }, part3.position);

        yield return MoveMultipleParts(new[] { part1, part2, part3 }, part4.position);

        Debug.Log("Door fully opened!");
    }

    private IEnumerator MovePartToTarget(Transform part, Vector3 target)
    {
        while (Vector3.Distance(part.position, target) > 0.01f)
        {
            part.position = Vector3.MoveTowards(part.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator MoveMultipleParts(Transform[] parts, Vector3 target)
    {
        bool allReached = false;

        while (!allReached)
        {
            allReached = true;

            foreach (Transform part in parts)
            {
                part.position = Vector3.MoveTowards(part.position, target, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(part.position, target) > 0.01f)
                {
                    allReached = false;
                }
            }

            yield return null;
        }
    }
}
