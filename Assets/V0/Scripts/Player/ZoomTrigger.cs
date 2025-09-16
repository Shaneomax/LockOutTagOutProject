using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Collider))]
public class ZoomTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera playerCamera;
    public float zoomFOV = 30f, zoomDuration = 0.5f;
    private float defaultFOV;

    [Header("Rotation Settings")]
    public Transform target;
    public float rotateDuration = 0.5f;

    [Header("References")]
    public PlayerController playerController;

    [Header("Step Tasks")]
    public List<StepInteractable> stepTasks = new List<StepInteractable>();

    [Header("Step Popups")]
    public List<string> stepPopupTexts = new List<string>();
    public TextMeshProUGUI popupTextMeshPro;
    public Vector3 popupOffset = new Vector3(50f, 50f, 0f);

    [Header("Audio + Subtitle Settings")]
    public List<SubtitleData> beforeTriggerSubtitleData = new List<SubtitleData>();
    public List<SubtitleData> afterTriggerSubtitleData = new List<SubtitleData>();

    [Header("Prompt Settings")]
    public List<GameObject> interactPrompts = new List<GameObject>();


    private int tasksCompleted;
    private bool isZoomed;
    private Canvas canvas;
    public static ZoomTrigger ActiveZoomTrigger;
    public System.Action<ZoomTrigger> onZoomOutCompleted;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        defaultFOV = playerCamera.fieldOfView;

        if (popupTextMeshPro)
        {
            popupTextMeshPro.gameObject.SetActive(false);
            canvas = popupTextMeshPro.GetComponentInParent<Canvas>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isZoomed || !other.CompareTag("Player") || !gameObject.activeSelf) return;

        playerController.canMove = false;
        InputManager.Instance.CanLook = false;

        if (target) RotatePlayerToTarget();
        else ZoomIn();
    }

    private void RotatePlayerToTarget()
    {
        Vector3 dir = target.position - playerController.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            playerController.transform.DORotateQuaternion(Quaternion.LookRotation(dir), rotateDuration);

        Vector3 camDir = target.position - playerController.playerCamera.transform.position;
        float pitch = -Mathf.Asin(camDir.normalized.y) * Mathf.Rad2Deg;
        playerController.playerCamera.transform.DOLocalRotateQuaternion(Quaternion.Euler(pitch, 0, 0), rotateDuration)
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

            TextManager.Instance.PlayWithSubtitles(
                FindObjectOfType<AudioSource>(),
                afterTriggerSubtitleData
            );

            ShowStepPopup();

            if (interactPrompts != null && interactPrompts.Count > 0)
            {
                for (int i = 0; i < interactPrompts.Count; i++)
                    interactPrompts[i].SetActive(i == 0);
            }
        });
    }


    private void ShowStepPopup()
    {
        if (!popupTextMeshPro || tasksCompleted >= stepTasks.Count) return;

        popupTextMeshPro.text = tasksCompleted < stepPopupTexts.Count
            ? stepPopupTexts[tasksCompleted]
            : "";

        Transform currentTarget = stepTasks[tasksCompleted].transform;
        Vector3 screenPos = playerCamera.WorldToScreenPoint(currentTarget.position);

        if (canvas)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos + popupOffset,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : playerCamera,
                out Vector2 localPoint
            );
            popupTextMeshPro.rectTransform.localPosition = localPoint;
        }

        popupTextMeshPro.gameObject.SetActive(true);

        for (int i = 0; i < interactPrompts.Count; i++)
            interactPrompts[i].SetActive(i == tasksCompleted);
    }


    private void HideStepPopup()
    {
        if (popupTextMeshPro != null)
        {
            popupTextMeshPro.gameObject.SetActive(false);
        }
    }

    public void RegisterTaskCompletion(StepInteractable task)
    {
        if (!isZoomed || !stepTasks.Contains(task)) return;

        HideStepPopup();

        if (tasksCompleted < interactPrompts.Count)
            interactPrompts[tasksCompleted].SetActive(false);

        tasksCompleted++;

        if (tasksCompleted < stepTasks.Count)
        {
            ShowStepPopup();
        }
        else
        {
            foreach (var prompt in interactPrompts)
                prompt.SetActive(false);

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


    public void PlayBeforeAudios() =>
        TextManager.Instance.PlayWithSubtitles(FindObjectOfType<AudioSource>(), beforeTriggerSubtitleData);
}
