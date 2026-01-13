using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("GameOverPanel thiếu CanvasGroup!");
        }
    }

    private void Start()
    {
        HideGameOver();
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);

        // 🔥 CHẶN TOÀN BỘ INPUT
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideGameOver()
    {
        gameOverPanel.SetActive(false);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
