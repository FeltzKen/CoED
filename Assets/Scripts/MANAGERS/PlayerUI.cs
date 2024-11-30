using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CoED;
using TMPro;

namespace CoED
{
    public class PlayerUI : MonoBehaviour
    {
        private static PlayerUI instance;

        public static PlayerUI Instance => instance;

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
        private TMP_Text healthText;

        [SerializeField]
        private Color healthBarBackground = Color.red;

        [SerializeField]
        private Color normalHealthColor = Color.green;

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
        private TMP_Text experienceText;

        [Header("Level UI")]
        [SerializeField]
        private TMP_Text levelText;

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
        [SerializeField]
        private TMP_Text staminaText;

        [Header("Magic UI")]
        [SerializeField]
        private Slider magicBar;
        [SerializeField]
        private TMP_Text magicText;

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
                Debug.LogWarning("PlayerUI instance already exists. Destroying duplicate.");
            }
        }
        private void Start()
        {
            PlayerStats.Instance.OnExperienceChanged += UpdateExperienceBar;
            PlayerStats.Instance.OnHealthChanged += UpdateHealthBar;
            PlayerStats.Instance.OnLevelUp += UpdateLevelDisplay;
            PlayerStats.Instance.OnMagicChanged += UpdateMagicBar;
            PlayerStats.Instance.OnStaminaChanged += UpdateStaminaBar;
            
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

        public void SetHealthBarMax(float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
            }
            if (healthText != null)
            {
                healthText.text = $"{maxHealth} / {maxHealth}";
            }
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
         
           if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{currentHealth} / {maxHealth}";
            }
                
            CheckLowHealth(currentHealth);
            
        }

        private void CheckLowHealth(float currentHealth)
        {
            float healthPercentage = currentHealth / healthBar.maxValue;
            if (healthPercentage < 0.9f && !isLowHealth)
            {
                isLowHealth = true;
                healthPulseCoroutine = StartCoroutine(PulseHealthBarBackground());
            }
            else if (healthPercentage >= 0.9f && isLowHealth)
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

        public void UpdateLevelDisplay()
        {
            if (levelText != null)
            {
                levelText.text = $"Level: {PlayerStats.Instance.level}";
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
            if (staminaText != null)
            {
                staminaText.text = $"{maxStamina} / {maxStamina}";
            }
        }


        public void UpdateStaminaBar(int currentStamina, int maxStamina)
        {
            if (staminaBar != null)
            {
                staminaBar.maxValue = maxStamina;
                staminaBar.value = currentStamina;
            }
            if (staminaText != null)
            {
                staminaText.text = $"{currentStamina} / {maxStamina}";
            }
        }

        public void SetMagicBarMax(float maxMagic)
        {
            if (magicBar != null)
            {
                magicBar.maxValue = maxMagic;
                magicBar.value = maxMagic;
            }
            if (magicText != null)
            {
                magicText.text = $"{maxMagic} / {maxMagic}";
            }
        }

        public void UpdateMagicBar(int currentMagic, int maxMagic)
        {
            if (magicBar != null)
            {
                magicBar.maxValue = maxMagic;
                magicBar.value = currentMagic;

            }
            if (magicText != null)
            {
                magicText.text = $"{currentMagic} / {maxMagic}";
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
