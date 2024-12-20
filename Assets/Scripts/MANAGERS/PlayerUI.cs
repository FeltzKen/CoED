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
        private Transform leftContainer;

        [SerializeField]
        private Transform rightContainer;

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
            if (spellPanel == null || leftContainer == null || rightContainer == null)
            {
                Debug.LogError("Spell panel or one of the containers is not assigned.");
                return;
            }

            if (spellButtonPrefab == null)
            {
                Debug.LogError("spellButtonPrefab is not assigned in the Inspector.");
                return;
            }

            // Clear containers
            foreach (Transform child in leftContainer)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in rightContainer)
            {
                Destroy(child.gameObject);
            }

            // Populate the containers
            foreach (PlayerSpell spell in availableSpells)
            {
                if (spell.selfTargeting)
                {
                    AddSpellToUI(spell, rightContainer); // Add to the right container
                }
                else
                {
                    AddSpellToUI(spell, leftContainer); // Add to the left container
                }
            }
        }

        private void AddSpellToUI(PlayerSpell spell, Transform targetSpellPanel)
        {
            if (spellUIMap.ContainsKey(spell))
            {
                Debug.LogWarning($"Spell {spell.spellName} is already in the UI.");
                return;
            }

            GameObject buttonObj = Instantiate(spellButtonPrefab, targetSpellPanel);
            buttonObj.name = $"{spell.spellName}_Button";

            Button spellButton = buttonObj.GetComponent<Button>();
            if (spellButton == null)
            {
                Debug.LogError("Spell button prefab is missing a Button component.");
                Destroy(buttonObj);
                return;
            }

            // Get the border image component
            Image borderImage = buttonObj.transform.Find("Border")?.GetComponent<Image>();
            if (borderImage == null)
            {
                Debug.LogError("Spell button prefab is missing a Border Image component.");
                Destroy(buttonObj);
                return;
            }
            borderImage.enabled = false; // Disable border by default
            // Get the base image component
            Image baseImage = buttonObj.transform.Find("BaseImage")?.GetComponent<Image>();
            if (baseImage == null)
            {
                Debug.LogError("Spell button prefab is missing a BaseImage component.");
                Destroy(buttonObj);
                return;
            }

            // Get the mask image component
            Image maskImage = buttonObj.transform.Find("MaskImage")?.GetComponent<Image>();
            if (maskImage == null)
            {
                Debug.LogError("Spell button prefab is missing a MaskImage component.");
                Destroy(buttonObj);
                return;
            }

            // Set the mask image type to filled and radial360
            maskImage.type = Image.Type.Filled;
            maskImage.fillMethod = Image.FillMethod.Radial360;

            // Assign the spell's icon to the base image
            if (spell.icon != null)
            {
                baseImage.sprite = spell.icon;
                baseImage.color = Color.white; // Set to white or any other default color
            }
            else
            {
                Debug.LogWarning($"Spell {spell.spellName} does not have an icon assigned.");
            }

            // Assign the spell to the button's click event
            spellButton.onClick.AddListener(() => OnSpellSelected(spell));

            // Create and store the UI element
            SpellUIElement uiElement = new SpellUIElement
            {
                button = spellButton,
                maskImage = maskImage,
                borderImage = borderImage,
                cooldownTimer = 0f,
            };

            spellUIMap.Add(spell, uiElement);
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
                uiElement.cooldownTimer = spell.cooldown; // Set cooldown timer
                uiElement.button.interactable = false;
                uiElement.maskImage.fillAmount = 0f; // Start with an empty mask

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
                yield return null; // Wait for the next frame
                uiElement.cooldownTimer -= Time.deltaTime;
                uiElement.maskImage.fillAmount = 1 - (uiElement.cooldownTimer / spell.cooldown);
            }

            // Cooldown completed
            uiElement.maskImage.fillAmount = 0f;
            uiElement.button.interactable = true;
        }

        public bool IsSpellOnCooldown(PlayerSpell spell)
        {
            if (spellUIMap.TryGetValue(spell, out SpellUIElement uiElement))
            {
                return uiElement.cooldownTimer > 0;
            }
            return false;
        }

        private void OnSpellSelected(PlayerSpell spell)
        {
            if (!spell.selfTargeting)
            {
                Debug.Log("spell is not self-targeting");
                HighlightSelectedSpell(spell);
                spellCaster.SelectSpell(spell);
            }
            else
            {
                Debug.Log("spell is self-targeting");
                spellCaster.CastSelfTargetingSpell(spell);
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

        private class SpellUIElement
        {
            public Button button;
            public Image maskImage; // Image component for the mask
            public Image borderImage; // Image component for the border
            public float cooldownTimer; // Cooldown timer in float
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
