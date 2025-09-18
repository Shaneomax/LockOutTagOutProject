using UnityEngine;
using System.Collections.Generic;

public class ZoomTriggerManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    public List<ZoomTrigger> triggers = new List<ZoomTrigger>();
    private int currentIndex = 0;

    private void Start()
    {
        for (int i = 0; i < triggers.Count; i++)
        {
            triggers[i].gameObject.SetActive(i == 0);
            triggers[i].onZoomOutCompleted = OnTriggerZoomOutCompleted;
        }

    }

    private void OnTriggerZoomOutCompleted(ZoomTrigger completedTrigger)
    {
        completedTrigger.gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex < triggers.Count)
        {
            triggers[currentIndex].gameObject.SetActive(true);
        }
    }

    public ZoomTrigger GetCurrentTrigger()
    {
        if (currentIndex < triggers.Count)
            return triggers[currentIndex];
        return null;
    }
}