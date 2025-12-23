using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1"); // tên scene màn 1
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2"); // tên scene màn 2
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3Scene"); // tên scene màn 3
    }
}
