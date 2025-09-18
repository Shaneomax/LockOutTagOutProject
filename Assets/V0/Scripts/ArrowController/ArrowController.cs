using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform arrow;
    public ZoomTriggerManager triggerManager;

    [Header("Settings")]
    public float arrowHeight = 2f;
    public float forwardOffset = 1f;

    private void Update()
    {
        if (player == null || arrow == null || triggerManager == null) return;

        // Position arrow above the player with forward offset
        arrow.position = player.position + Vector3.up * arrowHeight + player.forward * forwardOffset;

        ZoomTrigger targetTrigger = triggerManager.GetCurrentTrigger();

        if (targetTrigger != null)
        {
            if (!arrow.gameObject.activeSelf)
                arrow.gameObject.SetActive(true);

            Vector3 direction = (targetTrigger.transform.position - arrow.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                // Base look rotation
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                // Add 90 degrees on Z axis
                arrow.rotation = lookRotation * Quaternion.Euler(0f, 0f, 90f);
            }
        }
    }
}