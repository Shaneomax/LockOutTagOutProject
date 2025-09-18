using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class Step
{
    [Tooltip("The exact component that represents this step Interactables")]
    public List <StepInteractable> Interactables;

    public UnityEvent OnstepTrigger;
   

}


public class StepManager : MonoBehaviour
{
    public static StepManager Instance;
    public List<Step> steps = new List<Step>();
    public int currentStep = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //public bool IsCurrentStep(StepInteractable task)
    //{
    //    if (currentStep >= steps.Count)
    //    {
    //        steps[currentStep].OnstepTrigger.Invoke();
    //    }
    //    return steps[currentStep].Interactables == task;
    //}

    public void CompleteCurrentStep()
    {
        if (currentStep >= steps.Count) return;

        Debug.Log($"Step {currentStep + 1} completed!");
        currentStep++;

        if (currentStep >= steps.Count)
            Debug.Log("All steps completed!");
    }

    public void RegisterStep()
    {
        while (currentStep <= steps.Count)
        {
            steps[currentStep].OnstepTrigger.Invoke();
        }
    }
}