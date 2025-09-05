
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SlidingDoorSystem : MonoBehaviour
{
    [Header("Door Parts in Order")]
    public Transform part1;
    public Transform part2;
    public Transform part3;
    public Transform part4;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rayDistance = 5f;

    [Header("References")]
    public Camera playerCamera;
    public StepPuzzleSystem puzzleSystem;
    public LockSystem lockSystem; 

    private bool isOpening = false;

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed || isOpening) return;

        if (puzzleSystem == null || !puzzleSystem.IsDoorUnlocked())
        {
            Debug.Log("Door is locked!");
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                StartCoroutine(OpenDoor());
            }
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;

        yield return MovePartsTogether(new[] { part1 }, part2.position);
        yield return MovePartsTogether(new[] { part1, part2 }, part3.position);
        yield return MovePartsTogether(new[] { part1, part2, part3 }, part4.position);

        isOpening = false;

        Debug.Log("Door fully opened!");

        if (lockSystem != null)
        {
            lockSystem.EnableLockSystem();
        }
    }

    private IEnumerator MovePartsTogether(Transform[] parts, Vector3 target)
    {
        bool allReached = false;

        while (!allReached)
        {
            allReached = true;

            foreach (Transform part in parts)
            {
                part.position = Vector3.MoveTowards(part.position, target, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(part.position, target) > 0.01f)
                    allReached = false;
            }

            yield return null;
        }
    }
}
