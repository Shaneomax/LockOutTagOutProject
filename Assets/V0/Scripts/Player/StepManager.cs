using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    [Tooltip("The exact component that represents this step task")]
    public StepInteractable task;

    [Tooltip("Audio that will play before this step starts")]
    public AudioClip stepAudio;
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

    public bool IsCurrentStep(StepInteractable task)
    {
        if (currentStep >= steps.Count) return false;
        return steps[currentStep].task == task;
    }

    public void CompleteCurrentStep()
    {
        if (currentStep >= steps.Count) return;

        Debug.Log($"Step {currentStep + 1} completed!");
        currentStep++;

        if (currentStep >= steps.Count)
            Debug.Log("All steps completed!");
    }
}