using System.Collections;
using System.Collections.Generic;
using CoED;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerUI : MonoBehaviour
    {
        public static PlayerUI Instance => instance;
        private static PlayerUI instance;

        [Header("Magic Number UI")]
        [SerializeField]
        private Text magicNumberText;

        [Header("Step Count UI")]
        [SerializeField]
        private TextMeshProUGUI stepCountText;

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

        [Header("Spell UI")]
        [SerializeField]
        private Transform spellPanel;

        [SerializeField]
        private GameObject spellButtonPrefab;

        private PlayerSpellCaster spellCaster;

        [Header("Experience UI")]
        [SerializeField]
        private Slider experienceBar;

        [SerializeField]
        private TMP_Text experienceText;

        [Header("Level UI")]
        [SerializeField]
        private TMP_Text levelText;

        //[Header("Ability UI")]
        //[SerializeField]
        //private Image abilityIcon;

        //[SerializeField]
        //private Text abilityNameText;

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

        //[Header("Quest UI")]
        //[SerializeField]
        //private Text questTitleText;

        //[SerializeField]
        //private Text questDescriptionText;

        //[SerializeField]
        //private Text questProgressText;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                // Uncomment if needed
                DontDestroyOnLoad(gameObject);
                Debug.Log("PlayerSpellCaster: Instance initialized.");
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerUI instance already exists. Destroying duplicate.");
            }
        }

        private void Start()
        {
            spellCaster = PlayerSpellCaster.Instance;
            if (spellCaster == null)
            {
                Debug.LogError(
                    "PlayerSpellCaster instance is null. Ensure it's attached to the player prefab and present in the scene."
                );
                return;
            }

            PopulateSpellPanel();
        }

        private void PopulateSpellPanel()
        {
            if (spellPanel == null)
            {
                Debug.LogError("spellPanel is not assigned in the Inspector.");
                return;
            }

            if (spellButtonPrefab == null)
            {
                Debug.LogError("spellButtonPrefab is not assigned in the Inspector.");
                return;
            }

            foreach (var spell in spellCaster.AvailableSpells)
            {
                if (spell == null)
                {
                    Debug.LogWarning("Encountered a null spell in AvailableSpells.");
                    continue;
                }

                GameObject buttonObj = Instantiate(spellButtonPrefab, spellPanel);
                Button button = buttonObj.GetComponent<Button>();
                Image icon = buttonObj.GetComponentInChildren<Image>();

                if (icon != null)
                {
                    icon.sprite = spell.icon;
                }
                else
                {
                    Debug.LogWarning(
                        "Spell button prefab is missing an Image component for the icon."
                    );
                }

                if (button != null)
                {
                    button.onClick.AddListener(() => spellCaster.SelectSpell(spell));
                }
                else
                {
                    Debug.LogWarning("Spell button prefab is missing a Button component.");
                }
            }
        }

        // Update steps taken by adding 1 to the current step count
        public void UpdateStepCount()
        {
            if (stepCountText != null)
            {
                int currentSteps = int.Parse(stepCountText.text);
                stepCountText.text = (currentSteps + 1).ToString();
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

        /*
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
        */
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
            /*     if (questTitleText != null && questDescriptionText != null && questProgressText != null)
                 {
                     questTitleText.text = quest.QuestName;
                     questDescriptionText.text = quest.Description;
                     questProgressText.text = quest.GetProgressText();
                 }
            */
        }

        public void ShowQuestCompleted(Quest quest)
        {
            /*    if (questTitleText != null && questDescriptionText != null && questProgressText != null)
                {
                    questTitleText.text = $"{quest.QuestName} - Completed";
                    questDescriptionText.text = "Congratulations! You have completed the quest.";
                    questProgressText.text = "";
                }
            */
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
