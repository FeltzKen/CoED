using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class EnemyUI : MonoBehaviour, IStatusEffectSubscriber
    {
        public static EnemyUI enemyAI;
        public static _EnemyStats enemyStats;

        [Header("Health UI")]
        private Slider healthBar;
        private Transform statusEffectIconPanel;

        [SerializeField]
        private Color healthBarBackground = Color.red;

        [SerializeField]
        private Color normalHealthColor = Color.green;

        [SerializeField]
        private Color lowHealthColor = Color.magenta;

        [SerializeField]
        private float pulseSpeed = 1f;
        private bool isLowHealth = false;
        private Coroutine healthPulseCoroutine;

        private void Awake()
        {
            healthBar = GetComponentInChildren<Slider>();
            if (healthBar == null)
            {
                Debug.LogError("EnemyUI: HealthBar component not found.");
            }
            statusEffectIconPanel = transform.GetChild(0).GetChild(0);
            if (statusEffectIconPanel != null)
            {
                Debug.Log($"EnemyUI: StatusEffectIconPanel found at {statusEffectIconPanel.name}."); // Debugging
            }
            if (statusEffectIconPanel == null)
            {
                Debug.LogError("EnemyUI: StatusEffectIconPanel not found.");
            }
        }

        public void SetHealthBarMax(float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
            }
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
                CheckLowHealth(currentHealth, maxHealth);
            }
        }

        private void CheckLowHealth(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
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
                healthBarBackground = normalHealthColor;
            }
        }

        private IEnumerator PulseHealthBarBackground()
        {
            while (isLowHealth)
            {
                healthBarBackground = Color.Lerp(
                    normalHealthColor,
                    lowHealthColor,
                    Mathf.PingPong(Time.time * pulseSpeed, 1)
                );
                yield return null;
            }
        }

        // This method is called by the StatusEffectManager whenever effects change.using UnityEngine;


        /// <summary>
        /// Call this method when the active status effects on the entity change.
        /// It will refresh the icons displayed on the panel.
        /// </summary>
        public void OnStatusEffectsChanged(List<StatusEffect> activeEffects)
        {
            // Clear previous icons.
            foreach (Transform child in statusEffectIconPanel)
            {
                Destroy(child.gameObject);
            }

            // For each active effect, instantiate its icon prefab.
            foreach (StatusEffect effect in activeEffects)
            {
                if (effect.effectIconPrefab != null)
                {
                    // Instantiate the prefab as a child of the panel.
                    GameObject iconGO = Instantiate(
                        effect.effectIconPrefab,
                        statusEffectIconPanel,
                        false
                    );

                    // Optionally, if you want to assign the effect's sprite to an Image component:
                    Image iconImage = iconGO.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = effect.effectSprite;
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"Status effect {effect.effectName} does not have an icon prefab assigned."
                    );
                }
            }
        }
    }
}
