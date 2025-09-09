
using UnityEngine;

public class LightController : MonoBehaviour
{
    public bool startOn = false;

    private void Start()
    {
        ToggleLight(startOn);
    }

    public void ToggleLight(bool state)
    {
        gameObject.SetActive(state);
    }
}