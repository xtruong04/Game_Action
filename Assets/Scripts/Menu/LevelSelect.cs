using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private Text level1TimeText;
    [SerializeField] private Text level2TimeText;
    [SerializeField] private Text level3TimeText;

    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;

    void Start()
    {
        UpdateLevelUI();

        resetButton.onClick.AddListener(ResetProgress);
        // ? không b?t bu?c add backButton b?ng code
    }

    private void UpdateLevelUI()
    {
        float level1Time = PlayerPrefs.GetFloat("Level1_CompletionTime", 0);
        float level2Time = PlayerPrefs.GetFloat("Level2_CompletionTime", 0);
        float level3Time = PlayerPrefs.GetFloat("Level3_CompletionTime", 0);

        level1TimeText.text = level1Time > 0 ? $"Level 1: {level1Time:F2}s" : "Level 1: unfinished";
        level2TimeText.text = level2Time > 0 ? $"Level 2: {level2Time:F2}s" : "Level 2: unfinished";
        level3TimeText.text = level3Time > 0 ? $"Level 3: {level3Time:F2}s" : "Level 3: unfinished";

        level1Button.interactable = true;
        level2Button.interactable = level1Time > 0;
        level3Button.interactable = level2Time > 0;
    }

    public void LoadLevel1() => SceneManager.LoadScene("Level1");

    public void LoadLevel2()
    {
        if (PlayerPrefs.GetFloat("Level1_CompletionTime", 0) > 0)
            SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        if (PlayerPrefs.GetFloat("Level2_CompletionTime", 0) > 0)
            SceneManager.LoadScene("Level3");
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Level1_CompletionTime");
        PlayerPrefs.DeleteKey("Level2_CompletionTime");
        PlayerPrefs.DeleteKey("Level3_CompletionTime");
        PlayerPrefs.Save();

        UpdateLevelUI();
    }

    // ?? NÚT BACK
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
