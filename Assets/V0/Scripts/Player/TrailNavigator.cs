using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TrailNavigator : MonoBehaviour
{
    [Header("References")]
    public Transform player;                    // player position
    public ZoomTriggerManager triggerManager;   // your trigger manager

    [Header("Trail Settings")]
    public float updateRate = 0.2f; // how often to recalc path
    public float lineWidth = 0.2f;

    private LineRenderer lineRenderer;
    private NavMeshPath path;
    private float timer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;

        path = new NavMeshPath();
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

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);
        lineRenderer.SetPosition(1, target.transform.position + Vector3.up * 0.1f);
    }

}