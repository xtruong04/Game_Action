using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void SetMaxHealth(int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = max;
    }

    public void SetHealth(int current)
    {
        healthSlider.value = current;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
