using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ho?c TMPro n?u dùng TextMeshPro

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private Text level1TimeText;
    [SerializeField] private Text level2TimeText;
    [SerializeField] private Text level3TimeText;

    void Start()
    {
        // ??c th?i gian ?ã l?u t? PlayerPrefs
        float level1Time = PlayerPrefs.GetFloat("Level1_CompletionTime", 0);
        float level2Time = PlayerPrefs.GetFloat("Level2_CompletionTime", 0);
        float level3Time = PlayerPrefs.GetFloat("Level3Scene_CompletionTime", 0);

        // Hi?n th? lên UI (n?u có d? li?u)
        if (level1Time > 0) level1TimeText.text = "Level 1: " + level1Time.ToString("F2") + "s";
        else level1TimeText.text = "Level 1: unfinished";

        if (level2Time > 0) level2TimeText.text = "Level 2: " + level2Time.ToString("F2") + "s";
        else level2TimeText.text = "Level 2: unfinished";

        if (level3Time > 0) level3TimeText.text = "Level 3: " + level3Time.ToString("F2") + "s";
        else level3TimeText.text = "Level 3: unfinished";
    }

    public void LoadLevel1() => SceneManager.LoadScene("Level1");
    public void LoadLevel2() => SceneManager.LoadScene("Level2");
    public void LoadLevel3() => SceneManager.LoadScene("Level3Scene");
}
