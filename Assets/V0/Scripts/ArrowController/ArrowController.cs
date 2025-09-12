using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;                  
    public Transform arrow;                 
    public ZoomTriggerManager triggerManager; 

    [Header("Settings")]
    public float arrowHeight = 2f; 

    private void Update()
    {
        if (player == null || arrow == null || triggerManager == null) return;

        arrow.position = player.position + Vector3.up * arrowHeight;

        ZoomTrigger targetTrigger = triggerManager.GetCurrentTrigger();

        if (targetTrigger != null)
        {
            if (!arrow.gameObject.activeSelf)
                arrow.gameObject.SetActive(true);

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