using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        UpdateColor();
    }

    public void SetHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
        UpdateColor();
    }

    private void UpdateColor()
    {
        float percent = healthSlider.value / healthSlider.maxValue;
        if (percent > 0.5f)
            fillImage.color = Color.green;
        else if (percent > 0.25f)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.red;
    }
}
