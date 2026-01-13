using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private float startTime;

    void Start()
    {
        // L?u th?i gian b?t ??u màn
        startTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float finishTime = Time.time;
            float duration = finishTime - startTime;

            // L?u th?i gian hoàn thành màn hi?n t?i
            string currentLevel = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetFloat(currentLevel + "_CompletionTime", duration);

            // Load sang màn ch?n level
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
