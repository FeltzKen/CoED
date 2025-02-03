using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class TheElusiveShopGoblin : MonoBehaviour
    {
        public static TheElusiveShopGoblin Instance { get; private set; }

        [Header("Shop Settings")]
        [SerializeField]
        private int maxItemsForSale = 10;

        [SerializeField]
        private float curseRemovalPriceMultiplier = 2.5f;

        [SerializeField]
        private float negativeEffectRemovalPrice = 50f;

        [Header("Fair Sell Multiplier")]
        [SerializeField]
        private float sellPriceMultiplier = 0.75f;

        [Header("UI Panels")]
        [SerializeField]
        private GameObject panel; // Main shop panel

        [SerializeField]
        private Transform equipmentPanel;

        [SerializeField]
        private Transform consumablesPanel;

        [Header("Shop Buttons")]
        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private Button sellButton;

        [SerializeField]
        private Button removeCurseButton;

        [SerializeField]
        private Button removeEffectsButton;

        [SerializeField]
        private Button exitButton;

        [Header("Tabs & Money")]
        [SerializeField]
        private Button playerTabButton;

        [SerializeField]
        private Button shopTabButton;

        [SerializeField]
        private TextMeshProUGUI moneyValueText;

        [Header("Remove-Curse/Effect Window")]
        [SerializeField]
        private GameObject removeCurseOrEffectWindow;

        [SerializeField]
        private Button closeRemoveWindowButton;

        [Header("Dialogue & Labels")]
        [SerializeField]
        private TMP_Text goblinDialogueText;

        [Header("Item Stats Panel")]
        [SerializeField]
        private TextMeshProUGUI itemAttackValue;

        [SerializeField]
        private TextMeshProUGUI itemDefenseValue;

        [SerializeField]
        private TextMeshProUGUI itemMagicValue;

        [SerializeField]
        private TextMeshProUGUI itemHealthValue;

        [SerializeField]
        private TextMeshProUGUI itemStaminaValue;

        [SerializeField]
        private TextMeshProUGUI itemIntelligenceValue;

        [SerializeField]
        private TextMeshProUGUI itemDexterityValue;

        [SerializeField]
        private TextMeshProUGUI itemSpeedValue;

        [SerializeField]
        private TextMeshProUGUI itemCritChanceValue;

        [SerializeField]
        private TextMeshProUGUI StatusEffectsValue;

        [SerializeField]
        private TextMeshProUGUI StatusEffectsValueParent;

        [SerializeField]
        private TextMeshProUGUI resistancesValue;

        [SerializeField]
        private TextMeshProUGUI resistancesValueParent;

        [SerializeField]
        private TextMeshProUGUI weaknessesValue;

        [SerializeField]
        private TextMeshProUGUI weaknessesValueParent;

        [SerializeField]
        private TextMeshProUGUI itemStatsText;

        [SerializeField]
        private TextMeshProUGUI priceDisplayText;

        // --- Shop Data ---
        private Dictionary<int, Equipment> shopEquipmentInventory =
            new Dictionary<int, Equipment>();
        private Dictionary<int, ConsumableItem> shopConsumableInventory =
            new Dictionary<int, ConsumableItem>();
        private Dictionary<IShopItem, GameObject> shopItemButtons =
            new Dictionary<IShopItem, GameObject>();

        // --- Player References ---
        private EquipmentInventory playerEquipmentInventory;
        private ConsumableInventory playerConsumableInventory;
        private PlayerStats playerStats;

        // --- Selected Items ---
        private ConsumableItem selectedConsumable;
        private Equipment selectedEquipment;
        private GameObject selectedButton;
        private Dictionary<IShopItem, int> itemPrices = new Dictionary<IShopItem, int>();

        // --- Floor & Tab State ---
        private int currentFloorNumber;
        private bool isShopTabActive = true; // true => Shop Tab, false => Player Tab

        // *** For tracking whether we opened the remove window for "Curse" or "Effects"
        private bool removingCurse = false;
        private bool removingEffects = false;

        #region Unity Setup
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            playerEquipmentInventory = FindAnyObjectByType<EquipmentInventory>();
            playerConsumableInventory = FindAnyObjectByType<ConsumableInventory>();
            playerStats = PlayerStats.Instance;

            // Button events
            buyButton.onClick.AddListener(HandleBuy);
            sellButton.onClick.AddListener(HandleSell);
            removeCurseButton.onClick.AddListener(OnRemoveCurseButtonClicked);
            removeEffectsButton.onClick.AddListener(OnRemoveEffectsButtonClicked);
            exitButton.onClick.AddListener(CloseShop);

            playerTabButton.onClick.AddListener(OnPlayerTabButtonClicked);
            shopTabButton.onClick.AddListener(OnShopTabButtonClicked);

            closeRemoveWindowButton.onClick.AddListener(CloseRemoveCurseOrEffectWindow);

            removeCurseOrEffectWindow.SetActive(false);
            UpdateMoneyUI();
        }
        #endregion

        #region Shop Initialization
        public void InitializeShop(int floorNumber)
        {
            if (!isNearPlayer)
            {
                goblinDialogueText.text = "You must be near the shop goblin to shop!";
                return;
            }
            currentFloorNumber = floorNumber;
            GenerateShopInventory();
            PopulateShopUIPanels();

            goblinDialogueText.text = "Welcome, adventurer! Take a look at my wares!";
            UIPanelToggleManager.Instance.TogglePanel(panel);

            OnShopTabButtonClicked(); // default to the shop tab
        }

        private bool isNearPlayer
        {
            get
            {
                var player = PlayerStats.Instance.transform;
                var goblin = GameObject.Find("ShopGoblin").transform;
                return Vector2.Distance(player.position, goblin.position) < 2f;
            }
        }

        private void GenerateShopInventory()
        {
            shopEquipmentInventory.Clear();
            shopConsumableInventory.Clear();

            int tier = Mathf.Clamp(currentFloorNumber / 2, 1, 3);

            for (int i = 0; i < maxItemsForSale; i++)
            {
                Equipment eq = EquipmentGenerator.GenerateShopEquipment(tier);
                itemPrices[eq] = (int)PricingLibrary.CalculateEquipmentPrice(eq);

                shopEquipmentInventory.Add(i, eq);
            }
            for (int i = 0; i < maxItemsForSale; i++)
            {
                ConsumableItem c = ConsumableItemGenerator.GenerateRandomConsumable();
                itemPrices[c] = PricingLibrary.CalculateConsumablePrice(c);
                shopConsumableInventory.Add(i, c);
            }
        }

        private void PopulateShopUIPanels()
        {
            ClearUIPanels();
            shopItemButtons.Clear();

            foreach (var kvp in shopEquipmentInventory)
            {
                var eq = kvp.Value;
                AddItemToPanel(eq);
            }
            foreach (var kvp in shopConsumableInventory)
            {
                var c = kvp.Value;
                AddItemToPanel(c);
            }
        }
        #endregion

        #region Player Items
        private void PopulatePlayerPanels()
        {
            ClearUIPanels();
            shopItemButtons.Clear();

            var playerEquip = playerEquipmentInventory.GetAllEquipment();
            foreach (var eq in playerEquip)
                AddItemToPanel(eq);

            var playerCons = playerConsumableInventory.GetAllItems();
            foreach (var c in playerCons)
                AddItemToPanel(c);
        }

        private void ClearUIPanels()
        {
            if (equipmentPanel == null || consumablesPanel == null)
                return;

            foreach (Transform child in equipmentPanel)
                Destroy(child.gameObject);
            foreach (Transform child in consumablesPanel)
                Destroy(child.gameObject);
        }
        #endregion

        #region Remove-Curse/Effect Window (Open/Close)
        private void OnRemoveCurseButtonClicked()
        {
            removingCurse = true;
            removingEffects = false;

            DeselectCurrentlySelectedItem();
            buyButton.interactable = false;
            sellButton.interactable = false;

            goblinDialogueText.text = "Which item shall I remove curse for from?";
            removeCurseOrEffectWindow.SetActive(true);

            // Show the list of cursed items
            PopulateRemoveWindowWithCursedItems();
        }

        private void OnRemoveEffectsButtonClicked()
        {
            removingCurse = false;
            removingEffects = true;

            DeselectCurrentlySelectedItem();
            buyButton.interactable = false;
            sellButton.interactable = false;

            goblinDialogueText.text = "Which negative effect do you wish to remove?";
            removeCurseOrEffectWindow.SetActive(true);

            // Show the list of negative effects
            PopulateRemoveWindowWithEffects();
        }

        /// <summary>
        /// Clears any old children in the remove window, then populates with cursed items
        /// from the player's inventory.
        /// </summary>
        private void PopulateRemoveWindowWithCursedItems()
        {
            ClearRemoveWindow();

            // Grab any "Cursed" items from the player's inventory
            List<Equipment> cursedItems = playerEquipmentInventory
                .GetAllEquipment()
                .Where(eq => eq.itemName.Contains("Cursed"))
                .ToList();

            foreach (var item in cursedItems)
            {
                CreateRemoveButtonForItem(item);
            }
        }

        /// <summary>
        /// Clears old children in the remove window, then populates with negative status effects
        /// from the player's stats.
        /// </summary>
        private void PopulateRemoveWindowWithEffects()
        {
            ClearRemoveWindow();

            // Example: if "negative" means all active effects except buffs, or so
            List<StatusEffectType> negativeEffects = playerStats.activeStatusEffects;
            foreach (var effect in negativeEffects)
            {
                CreateRemoveButtonForEffect(effect);
            }
        }

        private void ClearRemoveWindow()
        {
            // Destroy any old UI under removeCurseOrEffectWindow
            foreach (Transform child in removeCurseOrEffectWindow.transform)
            {
                // skip the close button if it's a sibling
                if (child.name != closeRemoveWindowButton.name)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void CloseRemoveCurseOrEffectWindow()
        {
            removeCurseOrEffectWindow.SetActive(false);

            // Re-enable whichever button is correct based on the current tab
            if (isShopTabActive)
            {
                buyButton.interactable = true;
                sellButton.interactable = false;
            }
            else
            {
                buyButton.interactable = false;
                sellButton.interactable = true;
            }

            // Reset these
            removingCurse = false;
            removingEffects = false;
        }
        #endregion

        #region Creating Buttons for Items/Effects in the Remove Window
        private void CreateRemoveButtonForItem(Equipment item)
        {
            if (item == null)
                return;

            GameObject buttonObj = new GameObject($"Remove_{item.itemName}");
            buttonObj.transform.SetParent(removeCurseOrEffectWindow.transform, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(8f, 1f);

            // Add a background + text
            Image bg = buttonObj.AddComponent<Image>();
            bg.color = Color.red;

            // Add a Text child
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonObj.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(8f, 1f);
            var tmpText = textGO.AddComponent<TextMeshProUGUI>();
            tmpText.fontSize = .4f;
            tmpText.text = $"Remove curse from: {item.itemName}";
            tmpText.alignment = TMPro.TextAlignmentOptions.Center;

            // Add a Button
            Button btn = buttonObj.AddComponent<Button>();
            btn.onClick.AddListener(() => OnRemoveItemClicked(item));
        }

        private void CreateRemoveButtonForEffect(StatusEffectType effect)
        {
            GameObject buttonObj = new GameObject($"Remove_{effect}");
            buttonObj.transform.SetParent(removeCurseOrEffectWindow.transform, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(8f, 1f);

            // Add a background + text
            Image bg = buttonObj.AddComponent<Image>();
            bg.color = Color.red;

            // Add a Text child
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonObj.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(8f, 1f);
            var tmpText = textGO.AddComponent<TextMeshProUGUI>();
            tmpText.fontSize = .4f;
            tmpText.text = $"Remove: {effect}";
            tmpText.alignment = TMPro.TextAlignmentOptions.Center;

            // Add a Button
            Button btn = buttonObj.AddComponent<Button>();
            btn.onClick.AddListener(() => OnRemoveEffectClicked(effect));
        }

        private void OnRemoveItemClicked(Equipment cursedItem)
        {
            // Check if player can afford
            int cost = Mathf.RoundToInt(curseRemovalPriceMultiplier); // or scale by item tier, etc.
            if (playerStats.GetCurrency() < cost)
            {
                goblinDialogueText.text = "You can't afford to remove that curse!";
                return;
            }

            // Pay
            playerStats.SpendCurrency(cost);
            UpdateMoneyUI();

            // Actually remove "Cursed" from item
            cursedItem.itemName = cursedItem.itemName.Replace("Cursed", "Purified");
            // Or do deeper logic to remove the actual "isCursed" flag, etc.
            // Example: cursedItem.prePrefix = ""; cursedItem.isEnchantedOrCursed = false;

            goblinDialogueText.text = $"Curse removed from {cursedItem.itemName}!";

            // Refresh the window so that item is no longer listed
            if (removingCurse)
            {
                PopulateRemoveWindowWithCursedItems();
            }
        }

        private void OnRemoveEffectClicked(StatusEffectType effect)
        {
            // Check if player can afford
            int cost = Mathf.RoundToInt(negativeEffectRemovalPrice);
            if (playerStats.GetCurrency() < cost)
            {
                goblinDialogueText.text = "You can't afford to remove that effect!";
                return;
            }

            // Pay
            playerStats.SpendCurrency(cost);
            UpdateMoneyUI();

            // Actually remove effect from player
            playerStats.activeStatusEffects.Remove(effect);

            goblinDialogueText.text = $"Effect {effect} removed!";

            // Refresh the list so effect disappears
            if (removingEffects)
            {
                PopulateRemoveWindowWithEffects();
            }
        }
        #endregion

        #region Tabs
        private void OnPlayerTabButtonClicked()
        {
            isShopTabActive = false;
            DeselectCurrentlySelectedItem();

            PopulatePlayerPanels();
            buyButton.interactable = false;
            sellButton.interactable = true;

            goblinDialogueText.text = "Viewing your inventory...";
        }

        private void OnShopTabButtonClicked()
        {
            isShopTabActive = true;
            DeselectCurrentlySelectedItem();

            PopulateShopUIPanels();
            buyButton.interactable = true;
            sellButton.interactable = false;

            goblinDialogueText.text = "Viewing my fine wares...";
        }
        #endregion

        #region UI Button Creation for Shop/Player Items
        private void AddItemToPanel(IShopItem item)
        {
            if (item == null)
                return;

            Transform parentPanel = (item is Equipment) ? equipmentPanel : consumablesPanel;

            GameObject buttonObj = new GameObject($"{item.GetName()}");
            buttonObj.transform.SetParent(parentPanel, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(1f, 1f);

            GameObject background = new GameObject("Border");
            background.transform.SetParent(buttonObj.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(1f, 1f);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.green;
            background.SetActive(false);

            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(buttonObj.transform, false);
            RectTransform maskRect = mask.AddComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(1f, 1f);
            Image maskImage = mask.AddComponent<Image>();
            maskImage.color = Color.black;
            mask.SetActive(false);

            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(buttonObj.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(1f, 1f);
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = item.GetSprite();
            iconImage.preserveAspect = true;

            Button btn = buttonObj.AddComponent<Button>();
            btn.onClick.AddListener(() => OnItemClicked(item, buttonObj));

            shopItemButtons[item] = buttonObj;
        }

        private void BuildItemStats(ConsumableItem item)
        {
            itemAttackValue.text = item.consumableStats[Stat.Attack].ToString();
            itemDefenseValue.text = item.consumableStats[Stat.Defense].ToString();
            itemMagicValue.text = item.consumableStats[Stat.MaxMagic].ToString();
            itemHealthValue.text = item.consumableStats[Stat.MaxHP].ToString();
            itemStaminaValue.text = item.consumableStats[Stat.MaxStamina].ToString();
            itemIntelligenceValue.text = item.consumableStats[Stat.Intelligence].ToString();
            itemDexterityValue.text = item.consumableStats[Stat.Dexterity].ToString();
            itemSpeedValue.text = item.consumableStats[Stat.Speed].ToString();
            itemCritChanceValue.text = item.consumableStats[Stat.CritChance].ToString();

            resistancesValueParent.text = "Removed Effects";
            resistancesValue.text = string.Join(", ", item.removedEffects);
            weaknessesValueParent.text = "Added Effects";
            weaknessesValue.text = string.Join(", ", item.addedEffects);
            StatusEffectsValueParent.text = "Duration";
            StatusEffectsValue.text = item.hasDuration ? item.duration.ToString() : "";
        }

        private void BuildEquipmentStats(Equipment eq)
        {
            itemAttackValue.text = eq.equipmentStats[Stat.Attack].ToString();
            itemDefenseValue.text = eq.equipmentStats[Stat.Defense].ToString();
            itemMagicValue.text = eq.equipmentStats[Stat.MaxMagic].ToString();
            itemHealthValue.text = eq.equipmentStats[Stat.MaxHP].ToString();
            itemStaminaValue.text = eq.equipmentStats[Stat.MaxStamina].ToString();
            itemIntelligenceValue.text = eq.equipmentStats[Stat.Intelligence].ToString();
            itemDexterityValue.text = eq.equipmentStats[Stat.Dexterity].ToString();
            itemSpeedValue.text = eq.equipmentStats[Stat.Speed].ToString();
            itemCritChanceValue.text = eq.equipmentStats[Stat.CritChance].ToString();

            StatusEffectsValueParent.text = "Inflicted Effects";
            resistancesValueParent.text = "Resistances";
            weaknessesValueParent.text = "Weaknesses";
            StatusEffectsValue.text = string.Join(", ", eq.inflictedStatusEffects);
            resistancesValue.text = string.Join(", ", eq.resistanceEffects);
            weaknessesValue.text = string.Join(", ", eq.weaknessEffects);
        }

        #endregion

        #region Selection & Highlight
        private void OnItemClicked(IShopItem item, GameObject buttonObj)
        {
            DeselectCurrentlySelectedItem();

            selectedButton = buttonObj;
            if (item is Equipment eq)
            {
                selectedEquipment = eq;
                selectedConsumable = null;
                priceDisplayText.text = $"Price: {itemPrices[eq]} gold";
                BuildEquipmentStats(eq);
            }
            else if (item is ConsumableItem c)
            {
                selectedConsumable = c;
                selectedEquipment = null;
                priceDisplayText.text = $"Price: {itemPrices[c]} gold";
                BuildItemStats(c);
            }
            goblinDialogueText.text = $"Ah, a fine choice!\n {item.GetName()}";

            ShowButtonHighlight(buttonObj, true);
        }

        private void DeselectCurrentlySelectedItem()
        {
            if (selectedButton != null)
            {
                ShowButtonHighlight(selectedButton, false);
                selectedButton = null;
            }
            selectedEquipment = null;
            selectedConsumable = null;

            // Clear the stats panel
            if (itemStatsText != null)
                itemStatsText.text = "";
        }

        private void ShowButtonHighlight(GameObject buttonObj, bool show)
        {
            if (!buttonObj)
                return;

            var border = buttonObj.transform.Find("Border");
            if (border != null)
                border.gameObject.SetActive(show);

            var mask = buttonObj.transform.Find("Mask");
            if (mask != null)
                mask.gameObject.SetActive(show);
        }
        #endregion

        #region Buy / Sell
        public void HandleBuy()
        {
            IShopItem selectedItem = selectedEquipment ?? (IShopItem)selectedConsumable;
            if (selectedItem == null)
            {
                goblinDialogueText.text = "Please select an item first!";
                return;
            }

            int price =
                (selectedItem is Equipment eq)
                    ? (int)PricingLibrary.CalculateEquipmentPrice(eq)
                    : PricingLibrary.CalculateConsumablePrice((ConsumableItem)selectedItem);

            if (playerStats.GetCurrency() < price)
            {
                goblinDialogueText.text = "You don't have enough gold for that!";
                return;
            }

            // Remove from shop inventory
            if (selectedItem is Equipment)
                shopEquipmentInventory = shopEquipmentInventory
                    .Where(pair => pair.Value != selectedItem)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            else if (selectedItem is ConsumableItem)
                shopConsumableInventory = shopConsumableInventory
                    .Where(pair => pair.Value != selectedItem)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

            // Remove UI button
            if (shopItemButtons.TryGetValue(selectedItem, out GameObject button))
            {
                Destroy(button);
                shopItemButtons.Remove(selectedItem);
            }

            // Add to player inventory
            if (selectedItem is Equipment eqItem)
                playerEquipmentInventory.AddEquipment(eqItem);
            else if (selectedItem is ConsumableItem conItem)
                playerConsumableInventory.AddItem(conItem);

            playerStats.SpendCurrency(price);
            UpdateMoneyUI();
            goblinDialogueText.text = "Pleasure doing business with you!";

            // Refresh UI
            PopulateShopUIPanels();
        }

        public void HandleSell()
        {
            IShopItem selectedItem = selectedEquipment ?? (IShopItem)selectedConsumable;
            if (selectedItem == null)
            {
                goblinDialogueText.text = "Select something from your inventory to sell!";
                return;
            }

            int value = Mathf.RoundToInt(
                (selectedItem is Equipment eq)
                    ? PricingLibrary.CalculateEquipmentPrice(eq) * sellPriceMultiplier
                    : PricingLibrary.CalculateConsumablePrice((ConsumableItem)selectedItem)
                        * sellPriceMultiplier
            );

            // Remove from player inventory
            if (selectedItem is Equipment eqItem)
            {
                playerEquipmentInventory.RemoveEquipment(eqItem);
                shopEquipmentInventory.Add(shopEquipmentInventory.Count, eqItem);
            }
            else if (selectedItem is ConsumableItem conItem)
            {
                playerConsumableInventory.RemoveItem(conItem);
                shopConsumableInventory.Add(shopConsumableInventory.Count, conItem);
            }

            // Add item to shop UI
            AddItemToPanel(selectedItem);

            playerStats.GainCurrency(value);
            UpdateMoneyUI();
            goblinDialogueText.text = $"Sold for {value} gold!";

            // Refresh UI
            PopulatePlayerPanels();
        }

        #endregion

        #region Money UI
        private void UpdateMoneyUI()
        {
            if (moneyValueText != null)
            {
                moneyValueText.text = playerStats.GetCurrency().ToString();
            }
        }
        #endregion

        #region Close Shop & Teleport
        private void CloseShop()
        {
            UIPanelToggleManager.Instance.TogglePanel(panel);
            TeleportToNextFloor();
        }

        public void TeleportToNextFloor()
        {
            var goblin = GameObject.Find("ShopGoblin").transform;

            if (currentFloorNumber >= 7)
            {
                goblinDialogueText.text = "This is the final floor, my friend.";
                return;
            }

            currentFloorNumber++;
            FloorData nextFloor = DungeonManager.Instance.GetFloorData(currentFloorNumber);
            if (nextFloor == null)
            {
                Debug.LogError($"Floor {currentFloorNumber} not found!");
                return;
            }

            // Get tile position for goblin where no walls are present
            // for a 1 square radius
            var nextPositionList = nextFloor
                .GetRandomFloorTiles(currentFloorNumber)
                .Where(tile => !IsNearWall(tile, nextFloor, 1))
                .ToList();

            var nextPosition = nextPositionList[0];
            goblin.position = new Vector3(nextPosition.x, nextPosition.y, 0f);
            goblin.parent = DungeonManager.Instance.GetFloorTransform(currentFloorNumber).transform;

            // Reserve the tiles occupied by the goblin in the TileOccupationManager
            TileOccupancyManager.Instance.TryReserveTile(
                new Vector2Int((int)goblin.position.x, (int)goblin.position.y),
                -1002 // Goblin ID
            );
            TileOccupancyManager.Instance.TryReserveTile(
                new Vector2Int((int)goblin.position.x + 1, (int)goblin.position.y),
                -1002 // Goblin ID
            );
            TileOccupancyManager.Instance.TryReserveTile(
                new Vector2Int((int)goblin.position.x, (int)goblin.position.y + 1),
                -1002 // Goblin ID
            );
            TileOccupancyManager.Instance.TryReserveTile(
                new Vector2Int((int)goblin.position.x + 1, (int)goblin.position.y + 1),
                -1002 // Goblin ID
            );
            goblinDialogueText.text = "I've moved to the next floor. Come find me!";
            FloatingTextManager.Instance.ShowFloatingText(
                "Shop Goblin has moved to the next floor!",
                PlayerStats.Instance.transform,
                Color.yellow
            );
        }
        #endregion
        private bool IsNearWall(Vector2Int tile, FloorData floorData, int radius)
        {
            // A simple approach: check each wall tile, and if
            // distance <= radius, return true
            foreach (var wallTile in floorData.WallTiles)
            {
                if (Vector2Int.Distance(tile, wallTile) <= radius)
                {
                    return true; // It's near a wall
                }
            }
            return false; // No walls within radius => safe
        }
    }
}
