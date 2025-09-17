using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    [Header("Step Popups")]
    public List<string> stepPopupTexts = new List<string>();
    public GameObject popupObject; 
    private TextMeshProUGUI popupTextMeshPro;
    public Vector3 popupOffset = new Vector3(50f, 50f, 0f);

    [Header("Audio + Subtitle Settings")]
    public List<SubtitleData> beforeTriggerSubtitleData = new List<SubtitleData>();
    public List<SubtitleData> afterTriggerSubtitleData = new List<SubtitleData>();

    [Header("Outline Layer Settings")]
    public LayerMask outlineLayerMask;
    public LayerMask fallbackLayerMask;
    public bool restoreOriginalOnDisable = false;
    public bool setChildren = true;

    [Header("Subtitle Image")]
    public GameObject subtitleImage;

    private int outlineLayerIndex = -1;
    private int fallbackLayerIndex = 0;
    private Dictionary<Transform, int> originalLayerMap = new Dictionary<Transform, int>();

    private int tasksCompleted;
    private bool isZoomed;
    private Canvas canvas;

    public static ZoomTrigger ActiveZoomTrigger;
    public System.Action<ZoomTrigger> onZoomOutCompleted;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        defaultFOV = playerCamera.fieldOfView;

        if (popupObject)
        {
            popupTextMeshPro = popupObject.GetComponentInChildren<TextMeshProUGUI>();
            popupObject.SetActive(false);
            canvas = popupObject.GetComponentInParent<Canvas>();
        }

        outlineLayerIndex = GetFirstLayerIndex(outlineLayerMask);
        fallbackLayerIndex = GetFirstLayerIndex(fallbackLayerMask);
        if (fallbackLayerIndex < 0) fallbackLayerIndex = 0;

        CacheOriginalLayers();
        EnsureOutlineOffAtStart();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isZoomed || !other.CompareTag("Player") || !gameObject.activeSelf)
            return;

        playerController.canMove = false;
        InputManager.Instance.CanLook = false;

        if (target)
            RotatePlayerToTarget();
        else
            ZoomIn();
    }

    private void RotatePlayerToTarget()
    {
        Vector3 dir = target.position - playerController.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            playerController.transform.DORotateQuaternion(Quaternion.LookRotation(dir), rotateDuration);

        Vector3 camDir = target.position - playerController.playerCamera.transform.position;
        float pitch = -Mathf.Asin(camDir.normalized.y) * Mathf.Rad2Deg;
        playerController.playerCamera.transform
            .DOLocalRotateQuaternion(Quaternion.Euler(pitch, 0, 0), rotateDuration)
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

            PlaySubtitlesWithImage(FindObjectOfType<AudioSource>(), afterTriggerSubtitleData);

            if (outlineLayerIndex >= 0)
                EnableOutlineForCurrentTask();

            ShowStepPopup();
        });
    }

    private void ShowStepPopup()
    {
        if (!popupObject || !popupTextMeshPro || tasksCompleted >= stepTasks.Count)
            return;

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
            popupObject.GetComponent<RectTransform>().localPosition = localPoint;
        }

        popupObject.SetActive(true);
    }

    private void HideStepPopup()
    {
        if (popupObject != null)
            popupObject.SetActive(false);
    }

    public void RegisterTaskCompletion(StepInteractable task)
    {
        if (!isZoomed || !stepTasks.Contains(task))
            return;

        HideStepPopup();
        DisableOutlineForTask(task);

        tasksCompleted++;

        if (tasksCompleted < stepTasks.Count)
        {
            EnableOutlineForCurrentTask();
            ShowStepPopup();
        }
        else
        {
            DisableAllOutlineLayers();

            StartCoroutine(DelayedZoomOut());
        }
    }

    private IEnumerator DelayedZoomOut()
    {
        yield return new WaitForSeconds(0.7f); 

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


    public void PlayBeforeAudios() =>
        PlaySubtitlesWithImage(FindObjectOfType<AudioSource>(), beforeTriggerSubtitleData);

    private void PlaySubtitlesWithImage(AudioSource source, List<SubtitleData> subtitles)
    {
        if (subtitleImage != null)
            subtitleImage.SetActive(true);

        StartCoroutine(PlaySubtitlesCoroutine(source, subtitles));
    }

    private IEnumerator PlaySubtitlesCoroutine(AudioSource source, List<SubtitleData> subtitles)
    {
        yield return TextManager.Instance.PlayWithSubtitlesCoroutine(source, subtitles);

        if (subtitleImage != null)
            subtitleImage.SetActive(false);
    }

    private int GetFirstLayerIndex(LayerMask mask)
    {
        int m = mask.value;
        if (m == 0) return -1;

        for (int i = 0; i < 32; i++)
            if ((m & (1 << i)) != 0) return i;

        return -1;
    }

    private void CacheOriginalLayers()
    {
        originalLayerMap.Clear();
        if (stepTasks == null) return;

        foreach (var t in stepTasks)
        {
            if (t == null) continue;

            var transforms = t.GetComponentsInChildren<Transform>(true);
            foreach (var tr in transforms)
                if (!originalLayerMap.ContainsKey(tr))
                    originalLayerMap[tr] = tr.gameObject.layer;
        }
    }

    private void EnsureOutlineOffAtStart()
    {
        foreach (var t in stepTasks)
        {
            if (t == null) continue;
            if (setChildren)
                SetLayerRecursively(t.gameObject, fallbackLayerIndex);
            else
                t.gameObject.layer = fallbackLayerIndex;
        }
    }

    private void EnableOutlineForCurrentTask()
    {
        if (tasksCompleted >= stepTasks.Count) return;

        StepInteractable task = stepTasks[tasksCompleted];
        if (task == null) return;

        if (setChildren)
            SetLayerRecursively(task.gameObject, outlineLayerIndex);
        else
            task.gameObject.layer = outlineLayerIndex;
    }

    private void DisableOutlineForTask(StepInteractable task)
    {
        if (task == null) return;

        if (restoreOriginalOnDisable)
            RestoreOriginalLayersForTask(task);
        else
        {
            if (setChildren)
                SetLayerRecursively(task.gameObject, fallbackLayerIndex);
            else
                task.gameObject.layer = fallbackLayerIndex;
        }
    }

    private void DisableAllOutlineLayers()
    {
        foreach (var t in stepTasks)
        {
            if (t == null) continue;

            if (restoreOriginalOnDisable)
                RestoreOriginalLayersForTask(t);
            else
            {
                if (setChildren)
                    SetLayerRecursively(t.gameObject, fallbackLayerIndex);
                else
                    t.gameObject.layer = fallbackLayerIndex;
            }
        }
    }

    private void RestoreOriginalLayersForTask(StepInteractable task)
    {
        var transforms = task.GetComponentsInChildren<Transform>(true);
        foreach (var tr in transforms)
            if (originalLayerMap.TryGetValue(tr, out int original))
                tr.gameObject.layer = original;
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        var transforms = go.GetComponentsInChildren<Transform>(true);
        foreach (var tr in transforms)
            tr.gameObject.layer = layer;
    }
}
