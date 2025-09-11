using UnityEngine;
using System.Collections.Generic;

public class ZoomTriggerManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    public List<ZoomTrigger> triggers = new List<ZoomTrigger>();
    private int currentIndex = 0;

    private void Start()
    {
        // Deactivate all triggers except the first one
        for (int i = 0; i < triggers.Count; i++)
        {
            triggers[i].gameObject.SetActive(i == 0);
            triggers[i].onZoomOutCompleted = OnTriggerZoomOutCompleted;
        }
    }

    // Called by a trigger when its zoom-out finishes
    private void OnTriggerZoomOutCompleted(ZoomTrigger completedTrigger)
    {
        // Deactivate the completed trigger
        completedTrigger.gameObject.SetActive(false);

        // Move to next trigger
        currentIndex++;
        if (currentIndex < triggers.Count)
        {
            triggers[currentIndex].gameObject.SetActive(true);
        }
    }
}