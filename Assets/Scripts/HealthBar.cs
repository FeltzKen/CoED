using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        UpdateHealthText(maxHealth, maxHealth);
    }

    public void SetHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
        UpdateHealthText(currentHealth, (int)healthSlider.maxValue);
    }

    private void UpdateHealthText(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}