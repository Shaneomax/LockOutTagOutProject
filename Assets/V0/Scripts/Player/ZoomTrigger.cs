using UnityEngine;
using DG.Tweening;
using System.Collections;
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
                Vector3 direction = target.position - playerController.transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    float targetY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    Vector3 currentRotation = playerController.transform.eulerAngles;
                    Vector3 endRotation = new Vector3(currentRotation.x, targetY, currentRotation.z);

                    playerController.transform.DORotate(endRotation, rotateDuration, RotateMode.Fast)
                        .SetEase(Ease.OutSine)
                        .OnComplete(ZoomIn);
                }
                else
                {
                    ZoomIn();
                }
            }
            else
            {
                ZoomIn();
            }
        }
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
