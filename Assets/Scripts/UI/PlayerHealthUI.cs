using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Smooth Settings")]
    [SerializeField] private float smoothSpeed = 8f;

    private float targetHealth;
    private int maxHealth;

    void Update()
    {
        // Hiệu ứng tụt máu mượt
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            targetHealth,
            Time.unscaledDeltaTime * smoothSpeed
        );
    }

    public void SetMaxHealth(int max)
    {
        maxHealth = max;
        healthSlider.maxValue = max;
        healthSlider.minValue = 0;

        SetHealthInstant(max);
    }

    public void SetHealth(int current)
    {
        targetHealth = Mathf.Clamp(current, 0, maxHealth);
        UpdateText(targetHealth);

        // ⚠️ QUAN TRỌNG: giữ nguyên màu sprite asset
        fillImage.color = Color.white;
    }

    public void SetHealthInstant(int value)
    {
        targetHealth = value;
        healthSlider.value = value;
        UpdateText(value);
        fillImage.color = Color.white;
    }

    private void UpdateText(float hp)
    {
        if (hpText != null)
            hpText.text = $"{Mathf.RoundToInt(hp)} / {maxHealth}";
    }
}
