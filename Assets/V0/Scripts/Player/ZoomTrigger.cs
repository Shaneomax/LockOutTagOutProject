using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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

    [Header("Step Tasks")]
    public List<StepInteractable> stepTasks = new List<StepInteractable>();

    [Header("Audio Settings")]
    public List<AudioClip> beforeTriggerAudios = new List<AudioClip>();
    public List<AudioClip> afterTriggerAudios = new List<AudioClip>();

    private int tasksCompleted = 0;
    private bool isZoomed = false;
    public static ZoomTrigger ActiveZoomTrigger;

    public System.Action<ZoomTrigger> onZoomOutCompleted;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        defaultFOV = playerCamera.fieldOfView;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isZoomed && other.CompareTag("Player") && gameObject.activeSelf)
        {
            playerController.canMove = false;
            InputManager.Instance.CanLook = false;

            if (target != null)
            {
                RotatePlayerToTarget();
            }
            else
            {
                ZoomIn();
            }
        }
    }

    private void RotatePlayerToTarget()
    {

        Vector3 dirToTarget = target.position - playerController.transform.position;
        dirToTarget.y = 0f;
        if (dirToTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetYaw = Quaternion.LookRotation(dirToTarget);
            playerController.transform.DORotateQuaternion(targetYaw, rotateDuration);
        }

        Vector3 cameraDir = target.position - playerController.playerCamera.transform.position;
        float pitch = -Mathf.Asin(cameraDir.normalized.y) * Mathf.Rad2Deg;

        Quaternion camTarget = Quaternion.Euler(pitch, 0f, 0f);
        playerController.playerCamera.transform.DOLocalRotateQuaternion(camTarget, rotateDuration)
            .OnComplete(ZoomIn);
    }

    private void ZoomIn()
    {
        isZoomed = true;
        ActiveZoomTrigger = this;
        tasksCompleted = 0;

        playerCamera.DOFieldOfView(zoomFOV, zoomDuration).OnComplete(() =>
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            AudioManager.Instance.PlayAudioListAtPoint(afterTriggerAudios, transform.position);
        });
    }

    public void PlayBeforeAudios()
    {
        AudioManager.Instance.PlayAudioSequentially(
            FindObjectOfType<AudioSource>(), beforeTriggerAudios
        );
    }

    public void RegisterTaskCompletion(StepInteractable task)
    {
        if (!isZoomed || !stepTasks.Contains(task)) return;

        tasksCompleted++;

        if (tasksCompleted >= stepTasks.Count)
        {
            playerCamera.DOFieldOfView(defaultFOV, zoomDuration).OnComplete(() =>
            {
                isZoomed = false;
                ActiveZoomTrigger = null;

                playerController.canMove = true;
                InputManager.Instance.CanLook = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                onZoomOutCompleted?.Invoke(this);
            });
        }
    }
}
