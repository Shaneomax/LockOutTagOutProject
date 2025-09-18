using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider))]
public class ZoomTrigger : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineCamera defaultCam;
    public CinemachineCamera zoomCam;
    public float transitionDuration = 0.5f;

    [Header("References")]
    public PlayerController playerController;
    public GameObject subtitleImage;

    [Header("Step Tasks")]
    public List<StepInteractable> stepTasks = new();
    public List<string> stepPopupTexts = new();
    public GameObject popupObject;

    [Header("Popup Targets (Manual Assign)")]
    public List<Transform> popupTargets = new(); 
    public Vector3 popupOffset = new(0f, 2f, 0f); 

    [Header("Subtitles")]
    public List<SubtitleData> beforeTriggerSubtitleData = new();
    public List<SubtitleData> afterTriggerSubtitleData = new();

    [Header("Outline Layers")]
    public LayerMask outlineLayerMask;
    public LayerMask fallbackLayerMask;
    public bool restoreOriginalOnDisable = false;
    public bool setChildren = true;

    private TextMeshProUGUI popupTMP;
    private Canvas canvas;
    private int outlineLayerIndex, fallbackLayerIndex;
    private Dictionary<Transform, int> originalLayers = new();

    private int tasksCompleted;
    private bool isZoomed;

    private Transform currentPopupTarget;

    public static ZoomTrigger ActiveZoomTrigger;
    public System.Action<ZoomTrigger> onZoomOutCompleted;

    private void Awake()
    {
        if (popupObject)
        {
            popupTMP = popupObject.GetComponentInChildren<TextMeshProUGUI>();
            popupObject.SetActive(false);
            canvas = popupObject.GetComponentInParent<Canvas>();
        }

        outlineLayerIndex = GetFirstLayerIndex(outlineLayerMask);
        fallbackLayerIndex = Mathf.Max(0, GetFirstLayerIndex(fallbackLayerMask));

        CacheOriginalLayers();
        EnsureOutlineOffAtStart();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isZoomed || !other.CompareTag("Player")) return;

        playerController.canMove = false;
        InputManager.Instance.CanLook = false;

        ZoomIn();
    }

    private void ZoomIn()
    {
        isZoomed = true;
        ActiveZoomTrigger = this;
        tasksCompleted = 0;

        SwitchCamera(zoomCam, defaultCam, () =>
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlaySubtitlesWithImage(afterTriggerSubtitleData);

            if (outlineLayerIndex >= 0) EnableOutlineForCurrentTask();
            ShowStepPopup();
        });
    }

    private void ZoomOut()
    {
        SwitchCamera(defaultCam, zoomCam, () =>
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

    private void SwitchCamera(CinemachineCamera on, CinemachineCamera off, TweenCallback onComplete)
    {
        on.Priority = 20;
        off.Priority = 10;
        DOVirtual.DelayedCall(transitionDuration, onComplete);
    }

    private void ShowStepPopup()
    {
        if (!popupObject || !popupTMP || tasksCompleted >= stepTasks.Count) return;

        popupTMP.text = tasksCompleted < stepPopupTexts.Count ? stepPopupTexts[tasksCompleted] : "";

        // pick popup target from list
        if (tasksCompleted < popupTargets.Count)
            currentPopupTarget = popupTargets[tasksCompleted];
        else
            currentPopupTarget = stepTasks[tasksCompleted].transform; // fallback if not assigned

        popupObject.SetActive(true);
    }

    private void HideStepPopup()
    {
        popupObject?.SetActive(false);
        currentPopupTarget = null;
    }

    private void Update()
    {
        if (isZoomed && popupObject && currentPopupTarget)
            UpdatePopupPosition();
    }

    private void UpdatePopupPosition()
    {
        Vector3 worldPos = currentPopupTarget.position + popupOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (canvas && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPoint))
        {
            popupObject.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }

    public void RegisterTaskCompletion(StepInteractable task)
    {
        if (!isZoomed || !stepTasks.Contains(task)) return;

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
        yield return new WaitForSeconds(0.9f);
        ZoomOut();
    }

    public void PlayBeforeAudios() => PlaySubtitlesWithImage(beforeTriggerSubtitleData);

    private void PlaySubtitlesWithImage(List<SubtitleData> subtitles)
    {
        if (subtitleImage) subtitleImage.SetActive(true);
        StartCoroutine(PlaySubtitlesCoroutine(subtitles));
    }

    private IEnumerator PlaySubtitlesCoroutine(List<SubtitleData> subtitles)
    {
        yield return TextManager.Instance.PlayWithSubtitlesCoroutine(FindObjectOfType<AudioSource>(), subtitles);
        if (subtitleImage) subtitleImage.SetActive(false);
    }

    private int GetFirstLayerIndex(LayerMask mask)
    {
        int m = mask.value;
        if (m == 0) return -1;
        for (int i = 0; i < 32; i++) if ((m & (1 << i)) != 0) return i;
        return -1;
    }

    private void CacheOriginalLayers()
    {
        originalLayers.Clear();
        foreach (var t in stepTasks)
        {
            if (!t) continue;
            foreach (var tr in t.GetComponentsInChildren<Transform>(true))
                if (!originalLayers.ContainsKey(tr))
                    originalLayers[tr] = tr.gameObject.layer;
        }
    }

    private void EnsureOutlineOffAtStart()
    {
        foreach (var t in stepTasks)
            if (t) SetLayer(t.gameObject, fallbackLayerIndex);
    }

    private void EnableOutlineForCurrentTask()
    {
        if (tasksCompleted >= stepTasks.Count) return;
        SetLayer(stepTasks[tasksCompleted].gameObject, outlineLayerIndex);
    }

    private void DisableOutlineForTask(StepInteractable task)
    {
        if (!task) return;
        if (restoreOriginalOnDisable) RestoreLayers(task);
        else SetLayer(task.gameObject, fallbackLayerIndex);
    }

    private void DisableAllOutlineLayers()
    {
        foreach (var t in stepTasks)
        {
            if (!t) continue;
            if (restoreOriginalOnDisable) RestoreLayers(t);
            else SetLayer(t.gameObject, fallbackLayerIndex);
        }
    }

    private void RestoreLayers(StepInteractable task)
    {
        foreach (var tr in task.GetComponentsInChildren<Transform>(true))
            if (originalLayers.TryGetValue(tr, out int layer))
                tr.gameObject.layer = layer;
    }

    private void SetLayer(GameObject go, int layer)
    {
        foreach (var tr in go.GetComponentsInChildren<Transform>(true))
            tr.gameObject.layer = layer;
    }
}
