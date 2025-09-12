using UnityEngine;
using UnityEngine.SceneManagement;
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

    private string selectedMode = "";  

    private void Start()
    {
        formPanel.SetActive(true);
        choicePanel.SetActive(false);
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

 
    public void OnTraining()
    {
        selectedMode = "Training";
        Debug.Log("Training selected");
    }

    public void OnQualification()
    {
        selectedMode = "Qualification";
        Debug.Log("Qualification selected");
    }

    public void OnFinalSubmit()
    {
        if (string.IsNullOrEmpty(selectedMode))
        {
            return;
        }

        PlayerPrefs.SetString("GameMode", selectedMode);

        SceneManager.LoadScene("GameScene");
    }
}
