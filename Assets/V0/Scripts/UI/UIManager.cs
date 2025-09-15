using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject formPanel;
    public GameObject choicePanel;

    [Header("Input Fields")]
    public TMP_InputField nameInput;
    public TMP_InputField companyInput;
    public TMP_InputField emailInput;

    [Header("Mode Selection")]
    public Toggle trainingToggle;
    public Toggle qualificationToggle;

    private string selectedMode = "";

    private void Start()
    {
        formPanel.SetActive(true);
        choicePanel.SetActive(false);

        trainingToggle.isOn = false;
        qualificationToggle.isOn = false;

        trainingToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) selectedMode = "Training";
        });

        qualificationToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) selectedMode = "Qualification";
        });
    }

    public void OnFormSubmit()
    {
        string playerName = nameInput.text;
        string company = companyInput.text;
        string email = emailInput.text;

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("Company", company);
        PlayerPrefs.SetString("Email", email);

        formPanel.SetActive(false);
        choicePanel.SetActive(true);
    }

    public void OnBack()
    {
        choicePanel.SetActive(false);
        formPanel.SetActive(true);
    }

    public void OnFinalSubmit()
    {
        if (string.IsNullOrEmpty(selectedMode))
        {
            selectedMode = "Training";
        }

        Debug.Log(selectedMode + " selected, loading scene...");

        SceneManager.LoadScene("GameScene");
    }
}