using UnityEngine;
using UnityEngine.UI;
using CoED;
using System.Collections;
namespace CoED
{
    public class EnemyUI : MonoBehaviour
    {

        public static EnemyUI enemyAI;

        [Header("Health UI")]
        [SerializeField]
        private Slider healthBar;

        [SerializeField]
        private Image healthBarBackground;

        [SerializeField]
        private Color normalHealthColor = Color.red;

        [SerializeField]
        private Color lowHealthColor = new Color(0.6f, 0f, 0f, 1f);

        [SerializeField]
        private float pulseSpeed = 1f;
        private bool isLowHealth = false;
        private Coroutine healthPulseCoroutine;

        public void SetHealthBarMax(float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
            }
        }

        public void UpdateHealthBar(float currentHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth;
                CheckLowHealth(currentHealth);
            }
        }

        private void CheckLowHealth(float currentHealth)
        {
            float healthPercentage = currentHealth / healthBar.maxValue;
            if (healthPercentage < 0.2f && !isLowHealth)
            {
                isLowHealth = true;
                healthPulseCoroutine = StartCoroutine(PulseHealthBarBackground());
            }
            else if (healthPercentage >= 0.2f && isLowHealth)
            {
                isLowHealth = false;
                if (healthPulseCoroutine != null)
                {
                    StopCoroutine(healthPulseCoroutine);
                }
                healthBarBackground.color = normalHealthColor;
            }
        }

        private IEnumerator PulseHealthBarBackground()
        {
            while (isLowHealth)
            {
                healthBarBackground.color = Color.Lerp(
                    normalHealthColor,
                    lowHealthColor,
                    Mathf.PingPong(Time.time * pulseSpeed, 1)
                );
                yield return null;
            }
        }
    }
}