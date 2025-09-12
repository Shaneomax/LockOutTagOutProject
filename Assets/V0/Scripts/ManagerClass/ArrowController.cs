using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;                  // Player reference
    public Transform arrow;                   // Arrow object
    public ZoomTriggerManager triggerManager; // Drag your ZoomTriggerManager here

    [Header("Settings")]
    public float arrowHeight = 2f; // Height above player

    private void Update()
    {
        if (player == null || arrow == null || triggerManager == null) return;

        // Position arrow above player
        arrow.position = player.position + Vector3.up * arrowHeight;

        // Get current active trigger
        ZoomTrigger targetTrigger = triggerManager.GetCurrentTrigger();

        if (targetTrigger != null)
        {
            if (!arrow.gameObject.activeSelf)
                arrow.gameObject.SetActive(true);

            // Rotate arrow toward active trigger
            Vector3 targetPos = targetTrigger.transform.position;
            Vector3 direction = (targetPos - arrow.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
                arrow.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            if (arrow.gameObject.activeSelf)
                arrow.gameObject.SetActive(false);
        }
    }
}