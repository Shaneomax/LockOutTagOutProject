using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class LockSystem : MonoBehaviour
{
    [Header("Locks")]
    public Transform E1;
    public Transform P1;
    public Transform E2;
    public Transform P2;

    [Header("Cards")]
    public GameObject cardE1;
    public GameObject cardP1;
    public GameObject cardE2;
    public GameObject cardP2;

    [Header("Settings")]
    public float rotationSpeed = 90f;
    public float rayDistance = 5f;

    public Camera playerCamera;

    private bool lockSystemEnabled = false;

    private int currentStep = 0; 
    private Transform[] locks;
    private GameObject[] cards;
    private float[] zRotations;

    private void Awake()
    {
        locks = new Transform[] { E1, P1, E2, P2 };
        cards = new GameObject[] { cardE1, cardP1, cardE2, cardP2 };
        zRotations = new float[] { 90f, -90f, 90f, -90f };
    }

    public void EnableLockSystem()
    {
        lockSystemEnabled = true;
        Debug.Log("Lock system is now available!");
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!lockSystemEnabled)
        {
            Debug.Log("Lock system not active yet!");
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Lock"))
            {
                // Check if clicked lock is the correct one in sequence
                if (hit.transform == locks[currentStep])
                {
                    StartCoroutine(RotateLock(locks[currentStep], zRotations[currentStep], cards[currentStep]));
                    currentStep++;
                }
                else
                {
                    Debug.Log("Wrong lock! Follow the sequence E1 → P1 → E2 → P2.");
                }
            }
        }
    }

    private IEnumerator RotateLock(Transform lockPart, float targetZ, GameObject card)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);

        while (Quaternion.Angle(lockPart.localRotation, targetRotation) > 0.1f)
        {
            lockPart.localRotation = Quaternion.RotateTowards(
                lockPart.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        lockPart.localRotation = targetRotation;
        card.SetActive(true);
        Debug.Log(lockPart.name + " locked!");
    }
}
