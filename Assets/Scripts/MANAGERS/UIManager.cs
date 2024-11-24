using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CoED;

namespace CoED
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance => instance;

        [Header("Magic Number UI")]
        [SerializeField]
        private Text magicNumberText;

        [Header("Step Count UI")]
        [SerializeField]
        private Text stepCountText;

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

        [Header("Experience UI")]
        [SerializeField]
        private Slider experienceBar;

        [SerializeField]
        private Text experienceText;

        [Header("Level UI")]
        [SerializeField]
        private Text levelText;

        [Header("Ability UI")]
        [SerializeField]
        private Image abilityIcon;

        [SerializeField]
        private Text abilityNameText;

        [Header("Death UI")]
        [SerializeField]
        private GameObject deathPanel;

        [Header("Stamina UI")]
        [SerializeField]
        private Slider staminaBar;

        [Header("Magic UI")]
        [SerializeField]
        private Slider magicBar;

        [Header("Quest UI")]
        [SerializeField]
        private Text questTitleText;

        [SerializeField]
        private Text questDescriptionText;

        [SerializeField]
        private Text questProgressText;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("UIManager instance already exists. Destroying duplicate.");
            }
        }

        public void UpdateMagicNumber(int number)
        {
            if (magicNumberText != null)
            {
                magicNumberText.text = $"Magic Number: {number}";
            }
        }

        public void UpdateStepCount(int current, int total)
        {
            if (stepCountText != null)
            {
                stepCountText.text = $"Steps: {current}/{total}";
            }
        }

        public void SetHealthBarMax(int maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
            }
        }

        public void UpdateHealthBar(int currentHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth;
                CheckLowHealth(currentHealth);
            }
        }

        private void CheckLowHealth(int currentHealth)
        {
            float healthPercentage = (float)currentHealth / healthBar.maxValue;
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

        public void UpdateExperienceBar(int currentExp, int maxExp)
        {
            if (experienceBar != null)
            {
                experienceBar.maxValue = maxExp;
                experienceBar.value = currentExp;
            }
            if (experienceText != null)
            {
                experienceText.text = $"{currentExp} / {maxExp}";
            }
        }

        public void UpdateLevelDisplay(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Level: {level}";
            }
        }

        public void DisplayAbility(string abilityName, Sprite abilityIconSprite)
        {
            if (abilityNameText != null)
            {
                abilityNameText.text = abilityName;
            }
            if (abilityIcon != null)
            {
                abilityIcon.sprite = abilityIconSprite;
                abilityIcon.enabled = true;
            }
        }

        public void HideAbilityDisplay()
        {
            if (abilityNameText != null)
            {
                abilityNameText.text = "";
            }
            if (abilityIcon != null)
            {
                abilityIcon.sprite = null;
                abilityIcon.enabled = false;
            }
        }

        public void ShowDeathPanel()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
            }
        }

        public void HideDeathPanel()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(false);
            }
        }

        public void SetStaminaBarMax(float maxStamina)
        {
            if (staminaBar != null)
            {
                staminaBar.maxValue = maxStamina;
                staminaBar.value = maxStamina;
            }
        }

        public void UpdateStaminaBar(float currentStamina)
        {
            if (staminaBar != null)
            {
                staminaBar.value = currentStamina;
            }
        }

        public void SetMagicBarMax(float maxMagic)
        {
            if (magicBar != null)
            {
                magicBar.maxValue = maxMagic;
                magicBar.value = maxMagic;
            }
        }

        public void UpdateMagicBar(float currentMagic)
        {
            if (magicBar != null)
            {
                magicBar.value = currentMagic;
            }
        }

        public void ShowQuestAssigned(Quest quest)
        {
            if (questTitleText != null && questDescriptionText != null && questProgressText != null)
            {
                questTitleText.text = quest.QuestName;
                questDescriptionText.text = quest.Description;
                questProgressText.text = quest.GetProgressText();
            }
        }

        public void ShowQuestCompleted(Quest quest)
        {
            if (questTitleText != null && questDescriptionText != null && questProgressText != null)
            {
                questTitleText.text = $"{quest.QuestName} - Completed";
                questDescriptionText.text = "Congratulations! You have completed the quest.";
                questProgressText.text = "";
            }
        }

        public void OnSaveButtonClicked()
        {
            //  GameManager.Instance?.SaveGame();
        }

        public void OnLoadButtonClicked()
        {
            //GameManager.Instance?.LoadGame();
        }
    }
}
