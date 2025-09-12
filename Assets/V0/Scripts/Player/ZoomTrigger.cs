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

    // Delegate for manager
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
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        playerController.canMove = false;
        InputManager.Instance.CanLook = false;

        if (target != null)
        {
            Vector3 direction = (target.position - playerController.transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            playerController.transform.DORotateQuaternion(targetRotation, rotateDuration)
                .OnComplete(ZoomIn);
        }
        else
        {
            ZoomIn();
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

            // Play after-trigger audios here, after zoom in
            PlayAudioList(afterTriggerAudios);
        });
    }

    public void RegisterTaskCompletion(StepInteractable task)
    {
        if (!isZoomed || !stepTasks.Contains(task)) return;

        tasksCompleted++;

        if (tasksCompleted >= stepTasks.Count)
        {
            ZoomOut();
        }
    }

    private void ZoomOut()
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

            // No need to play afterTriggerAudios here anymore
            onZoomOutCompleted?.Invoke(this);
        });
    }

    public void PlayBeforeAudios(AudioSource source)
    {
        if (beforeTriggerAudios.Count > 0 && source != null)
        {
            StartCoroutine(PlayAudioSequentially(source, beforeTriggerAudios));
        }
    }

    private void PlayAudioList(List<AudioClip> clips)
    {
        foreach (var clip in clips)
        {
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    private System.Collections.IEnumerator PlayAudioSequentially(AudioSource source, List<AudioClip> clips)
    {
        foreach (var clip in clips)
        {
            source.clip = clip;
            source.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }
}
