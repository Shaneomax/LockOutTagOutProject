using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class ZoomTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float zoomFOV = 30f;
    public float zoomDuration = 0.5f;
    private float defaultFOV;

    [Header("Rotation Settings")]
    public Transform target;
    public float rotateDuration = 0.5f;

    [Header("References")]
    public PlayerController playerController;

    private bool isZoomed = false;
    public static ZoomTrigger ActiveZoomTrigger;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        defaultFOV = playerCamera.fieldOfView;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isZoomed)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        if (target == null)
        {
            ZoomIn();
            return;
        }

        playerController.canMove = false;
        InputManager.Instance.CanLook = false;

        Vector3 lookDir = (target.position - playerController.transform.position).normalized;
        lookDir.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(lookDir);

        playerController.transform.DORotateQuaternion(targetRot, rotateDuration)
            .OnComplete(() => ZoomIn());
    }

    private void ZoomIn()
    {
        isZoomed = true;
        ActiveZoomTrigger = this;

        playerCamera.DOFieldOfView(zoomFOV, zoomDuration).OnComplete(() =>
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        });
    }

    public void ZoomOut()
    {
        if (!isZoomed) return;

        playerCamera.DOFieldOfView(defaultFOV, zoomDuration).OnComplete(() =>
        {
            isZoomed = false;
            ActiveZoomTrigger = null;

            playerController.canMove = true;
            InputManager.Instance.CanLook = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        });
    }
}
