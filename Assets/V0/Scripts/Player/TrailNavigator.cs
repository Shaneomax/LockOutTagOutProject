using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class TrailNavigator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public ZoomTriggerManager triggerManager;

    [Header("Trail Settings")]
    public float updateRate = 0.2f;
    public float lineWidth = 0.2f;
    public float tweenDuration = 0.3f; 

    private LineRenderer lineRenderer;
    private float timer;

    private Tween startTween;
    private Tween endTween;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateRate)
        {
            timer = 0f;
            UpdateTrail();
        }
    }

    void UpdateTrail()
    {
        ZoomTrigger target = triggerManager.GetCurrentTrigger();
        if (target == null || player == null)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        if (lineRenderer.positionCount != 2)
            lineRenderer.positionCount = 2;

        startTween?.Kill();
        endTween?.Kill();

        Vector3 newStart = player.position + Vector3.up * 0.1f;
        startTween = DOTween.To(
            () => lineRenderer.GetPosition(0),
            pos => lineRenderer.SetPosition(0, pos),
            newStart,
            tweenDuration
        ).SetEase(Ease.Linear);

        Vector3 newEnd = target.transform.position + Vector3.up * 0.1f;
        endTween = DOTween.To(
            () => lineRenderer.GetPosition(1),
            pos => lineRenderer.SetPosition(1, pos),
            newEnd,
            tweenDuration
        ).SetEase(Ease.Linear);
    }
}
