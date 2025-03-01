using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerUI : MonoBehaviour, IStatusEffectSubscriber
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
        private Transform statusEffectIconPanel;

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

        private PlayerSpellCaster spellCaster;
        private Dictionary<Spell, SpellUIElement> spellUIMap =
            new Dictionary<Spell, SpellUIElement>();

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
        }

        public void AddSpell(Spell spell)
        {
            AddSpellToUI(spell, spell.SelfTargeting ? rightContainer : leftContainer);
        }

        private void AddSpellToUI(Spell spell, Transform targetSpellPanel)
        {
            if (spellUIMap.ContainsKey(spell))
            {
                Debug.LogWarning($"Spell {spell.SpellName} is already in the UI.");
                return;
            }

            GameObject buttonObj = Instantiate(spellButtonPrefab, targetSpellPanel);
            buttonObj.name = $"{spell.SpellName}_Button";

            Button spellButton = buttonObj.GetComponent<Button>();
            if (spellButton == null)
            {
                Debug.LogError("Spell button prefab is missing a Button component.");
                Destroy(buttonObj);
                return;
            }

            Image borderImage = buttonObj.transform.Find("Border")?.GetComponent<Image>();
            if (borderImage == null)
            {
                Debug.LogError("Spell button prefab is missing a Border Image component.");
                Destroy(buttonObj);
                return;
            }
            borderImage.enabled = false;
            Image baseImage = buttonObj.transform.Find("BaseImage")?.GetComponent<Image>();
            if (baseImage == null)
            {
                Debug.LogError("Spell button prefab is missing a BaseImage component.");
                Destroy(buttonObj);
                return;
            }

            Image maskImage = buttonObj.transform.Find("MaskImage")?.GetComponent<Image>();
            if (maskImage == null)
            {
                Debug.LogError("Spell button prefab is missing a MaskImage component.");
                Destroy(buttonObj);
                return;
            }

            maskImage.type = Image.Type.Filled;
            maskImage.fillMethod = Image.FillMethod.Radial360;

            if (spell.Icon != null)
            {
                baseImage.sprite = spell.Icon;
                baseImage.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"Spell {spell.SpellName} does not have an icon assigned.");
            }

            spellButton.onClick.AddListener(() => OnSpellSelected(spell));

            SpellUIElement uiElement = new SpellUIElement
            {
                button = spellButton,
                maskImage = maskImage,
                borderImage = borderImage,
                cooldownTimer = 0f,
            };

            Debug.Log($"Added spell {spell.SpellName} to UI.");
            spellUIMap.Add(spell, uiElement);
        }

        private void HighlightSelectedSpell(Spell selectedSpell)
        {
            foreach (var pair in spellUIMap)
            {
                if (pair.Key == selectedSpell)
                {
                    pair.Value.borderImage.enabled = true;
                }
                else
                {
                    pair.Value.borderImage.enabled = false;
                }
            }
        }

        public void OnSpellCast(Spell spell)
        {
            if (spellUIMap.TryGetValue(spell, out SpellUIElement uiElement))
            {
                uiElement.cooldownTimer = spell.Cooldown;
                uiElement.button.interactable = false;
                uiElement.maskImage.fillAmount = 0f;

                StartCoroutine(CooldownCoroutine(spell, uiElement));
            }

            UpdateMagicBar(
                PlayerStats.Instance.GetCurrentMagic(),
                PlayerStats.Instance.GetCurrentMaxMagic()
            );
        }

        private IEnumerator CooldownCoroutine(Spell spell, SpellUIElement uiElement)
        {
            while (uiElement.cooldownTimer > 0)
            {
                yield return null;
                uiElement.cooldownTimer -= Time.deltaTime;
                uiElement.maskImage.fillAmount = 1 - (uiElement.cooldownTimer / spell.Cooldown);
            }

            uiElement.maskImage.fillAmount = 0f;
            uiElement.button.interactable = true;
        }

        public bool IsSpellOnCooldown(Spell spell)
        {
            if (spellUIMap.TryGetValue(spell, out SpellUIElement uiElement))
            {
                return uiElement.cooldownTimer > 0;
            }
            return false;
        }

        private void OnSpellSelected(Spell spell)
        {
            if (!spell.SelfTargeting)
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

        public void UpdateMagicBar(float currentMagic, float maxMagic)
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
            public Image maskImage;
            public Image borderImage;
            public float cooldownTimer;
        }

        public void UpdateStepCount()
        {
            if (stepCountText != null)
            {
                stepCountText.text = PlayerStats.Instance.steps.ToString();
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

        public void UpdateHealthBar(float currentHealth, float maxHealth)
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

        public void UpdateExperienceBar(float currentExp, float maxExp)
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

        public void UpdateStaminaBar(float currentStamina, float maxStamina)
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

        public void UpdateUIPanels()
        {
            UpdateHealthBar(
                PlayerStats.Instance.GetCurrentHealth(),
                PlayerStats.Instance.GetCurrentMaxHealth()
            );
            UpdateStaminaBar(
                PlayerStats.Instance.GetCurrentStamina(),
                PlayerStats.Instance.GetCurrentMaxStamina()
            );
            UpdateMagicBar(
                PlayerStats.Instance.GetCurrentMagic(),
                PlayerStats.Instance.GetCurrentMaxMagic()
            );
        }

        public void OnStatusEffectsChanged(List<StatusEffect> activeEffects)
        {
            // Clear previous icons.
            foreach (Transform child in statusEffectIconPanel)
            {
                Destroy(child.gameObject);
            }

            // For each active effect, create a new icon GameObject
            foreach (StatusEffect effect in activeEffects)
            {
                // Create a new GameObject to hold the icon.
                GameObject iconGO = new GameObject(effect.effectName + "_Icon");
                // Set its parent so it appears in the icon panel.
                iconGO.transform.SetParent(statusEffectIconPanel, false);

                // Optionally, add a RectTransform if you need layout options.
                RectTransform rect = iconGO.AddComponent<RectTransform>();
                // You can set default size if needed (example: 32x32)
                rect.sizeDelta = new Vector2(32, 32);

                // Add the Image component to the GameObject.
                Image iconImage = iconGO.AddComponent<Image>();
                // Set the Image's sprite to the effect's sprite.
                iconImage.sprite = effect.effectSprite;

                // (Optional) Set additional properties on the Image (like preserving aspect ratio)
                iconImage.preserveAspect = true;
            }
        }
    }
}
