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
        public static PlayerUI Instance { get; private set; }

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

        [Header("Available Spells")]
        [SerializeField]
        private List<PlayerSpell> availableSpells;

        private PlayerSpellCaster spellCaster;
        private Dictionary<PlayerSpell, SpellUIElement> spellUIMap =
            new Dictionary<PlayerSpell, SpellUIElement>();

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
            if (Instance == null)
            {
                Instance = this;
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

            foreach (PlayerSpell spell in availableSpells)
            {
                AddSpellToUI(spell);
            }
        }

        private void AddSpellToUI(PlayerSpell spell)
        {
            if (spellUIMap.ContainsKey(spell))
            {
                Debug.LogWarning($"Spell {spell.spellName} is already in the UI.");
                return;
            }

            GameObject buttonObj = Instantiate(spellButtonPrefab, spellPanel);
            buttonObj.name = $"{spell.spellName}_Button";

            Button spellButton = buttonObj.GetComponent<Button>();
            if (spellButton == null)
            {
                Debug.LogError("Spell button prefab is missing a Button component.");
                Destroy(buttonObj);
                return;
            }

            // Assign the spell's icon to the button's Image component
            if (spell.icon != null)
            {
                spellButton.image.sprite = spell.icon;
            }
            else
            {
                Debug.LogWarning($"Spell {spell.spellName} does not have an icon assigned.");
            }

            // Get the cooldown text component
            TextMeshProUGUI cooldownText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (cooldownText == null)
            {
                Debug.LogError(
                    "Spell button prefab is missing a TextMeshProUGUI component for cooldown text."
                );
                Destroy(buttonObj);
                return;
            }

            // Get the border Image component
            Image borderImage = buttonObj.transform.Find("Border")?.GetComponent<Image>();
            if (borderImage == null)
            {
                Debug.LogError("Spell button prefab is missing a Border Image component.");
                Destroy(buttonObj);
                return;
            }
            borderImage.enabled = false; // Initially disable the border

            // Assign the spell to the button's click event
            spellButton.onClick.AddListener(() => OnSpellSelected(spell));

            // Create and store the UI element
            SpellUIElement uiElement = new SpellUIElement
            {
                button = spellButton,
                cooldownText = cooldownText,
                borderImage = borderImage,
                cooldownTimer = 0,
            };

            spellUIMap.Add(spell, uiElement);
        }

        public void UpdateSpellList(List<PlayerSpell> newSpells)
        {
            foreach (var spell in newSpells)
            {
                if (!availableSpells.Contains(spell))
                {
                    availableSpells.Add(spell);
                    AddSpellToUI(spell);
                }
            }
        }

        private void OnSpellSelected(PlayerSpell spell)
        {
            spellCaster.SelectSpell(spell);
            HighlightSelectedSpell(spell);
        }

        private void HighlightSelectedSpell(PlayerSpell selectedSpell)
        {
            foreach (var pair in spellUIMap)
            {
                if (pair.Key == selectedSpell)
                {
                    pair.Value.borderImage.enabled = true; // Enable border for selected spell
                }
                else
                {
                    pair.Value.borderImage.enabled = false; // Disable border for other spells
                }
            }
        }

        public void OnSpellCast(PlayerSpell spell)
        {
            if (spellUIMap.TryGetValue(spell, out SpellUIElement uiElement))
            {
                uiElement.cooldownTimer = (int)spell.cooldown; // Set cooldown timer
                uiElement.button.interactable = false;
                uiElement.cooldownText.text = uiElement.cooldownTimer.ToString();
                SetButtonAlpha(uiElement.button, 0.2f); // Start faded

                // Start the cooldown coroutine
                StartCoroutine(CooldownCoroutine(spell, uiElement));
            }

            // Update the magic bar
            UpdateMagicBar(PlayerStats.Instance.CurrentMagic, PlayerStats.Instance.MaxMagic);
        }

        private IEnumerator CooldownCoroutine(PlayerSpell spell, SpellUIElement uiElement)
        {
            while (uiElement.cooldownTimer > 0)
            {
                yield return new WaitForSeconds(1f);
                uiElement.cooldownTimer -= 1;
                uiElement.cooldownText.text = uiElement.cooldownTimer.ToString();

                // Update fade effect
                float alpha = Mathf.Lerp(0.2f, 1f, (float)uiElement.cooldownTimer / spell.cooldown);
                SetButtonAlpha(uiElement.button, alpha);
            }

            // Cooldown completed
            uiElement.cooldownText.text = "";
            uiElement.button.interactable = true;
            SetButtonAlpha(uiElement.button, 1f); // Reset alpha
        }

        private void SetButtonAlpha(Button button, float alpha)
        {
            // Update the alpha of the button's image
            Color color = button.image.color;
            color.a = alpha;
            button.image.color = color;

            // Update the alpha of child images (e.g., icon)
            Image[] childImages = button.GetComponentsInChildren<Image>();
            foreach (var img in childImages)
            {
                color = img.color;
                color.a = alpha;
                img.color = color;
            }

            // Update the alpha of the cooldown text
            TextMeshProUGUI[] texts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        public bool IsSpellOnCooldown(PlayerSpell spell)
        {
            if (spellUIMap.TryGetValue(spell, out SpellUIElement uiElement))
            {
                return uiElement.cooldownTimer > 0;
            }
            return false;
        }

        private class SpellUIElement
        {
            public Button button;
            public TextMeshProUGUI cooldownText;
            public Image borderImage; // Image component for the border
            public int cooldownTimer; // Cooldown timer in integer steps
        }

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
