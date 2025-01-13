using UnityEngine;

namespace CoED
{
    public class EquipmentWrapper : Equipment
    {
        [Header("Equipment Data")]
        public Equipment equipmentData;

        [Header("Dynamic Modifiers")]
        public float enchantedAttackModifier;
        public float enchantedDefenseModifier;
        public float enchantedHealthModifier;
        public float enchantedSpeedModifier;
        public float enchantedMagicModifier;
        public float enchantedStaminaModifier;

        public float cursedAttackModifier;
        public float cursedDefenseModifier;
        public float cursedHealthModifier;
        public float cursedSpeedModifier;
        public float cursedMagicModifier;
        public float cursedStaminaModifier;

        [Header("Runtime State")]
        public bool IsEnchanted = false;
        public bool IsCursed = false;
        public bool hasBeenRevealed = false;

        public void Initialize(Equipment data)
        {
            equipmentData = Instantiate<Equipment>(data);
        }

        public void RevealItem()
        {
            if (hasBeenRevealed)
                return;
            hasBeenRevealed = true;

            if (IsEnchanted)
                equipmentData.itemName = $"Enchanted {equipmentData.itemName}";
            else if (IsCursed)
                equipmentData.itemName = $"Cursed {equipmentData.itemName}";

            Debug.Log($"{equipmentData.equipmentName} has been revealed.");
        }

        public void RemoveCurse()
        {
            IsCursed = false;
            Debug.Log($"{equipmentData.equipmentName} is no longer cursed.");
        }

        public float GetTotalAttackModifier() =>
            equipmentData.attackModifier + enchantedAttackModifier - cursedAttackModifier;

        public float GetTotalDefenseModifier() =>
            equipmentData.defenseModifier + enchantedDefenseModifier - cursedDefenseModifier;

        public float GetTotalHealthModifier() =>
            equipmentData.healthModifier + enchantedHealthModifier - cursedHealthModifier;

        public float GetTotalSpeedModifier() =>
            equipmentData.speedModifier + enchantedSpeedModifier - cursedSpeedModifier;

        public float GetTotalMagicModifier() =>
            equipmentData.magicModifier + enchantedMagicModifier - cursedMagicModifier;

        public float GetTotalStaminaModifier() =>
            equipmentData.staminaModifier + enchantedStaminaModifier - cursedStaminaModifier;

        public void ApplyStatModifiers(int scaledFactor)
        {
            if (equipmentData == null)
            {
                Debug.LogError("EquipmentWrapper: Equipment data is missing!");
                return;
            }

            if (equipmentData.attackModifier > 0)
                equipmentData.attackModifier +=
                    scaledFactor + enchantedAttackModifier - cursedAttackModifier;
            if (equipmentData.defenseModifier > 0)
                equipmentData.defenseModifier +=
                    scaledFactor + enchantedDefenseModifier - cursedDefenseModifier;
            if (equipmentData.healthModifier > 0)
                equipmentData.healthModifier +=
                    scaledFactor + enchantedHealthModifier - cursedHealthModifier;
            if (equipmentData.speedModifier > 0)
                equipmentData.speedModifier +=
                    scaledFactor + enchantedSpeedModifier - cursedSpeedModifier;
            if (equipmentData.magicModifier > 0)
                equipmentData.magicModifier +=
                    scaledFactor + enchantedMagicModifier - cursedMagicModifier;
            if (equipmentData.staminaModifier > 0)
                equipmentData.staminaModifier +=
                    scaledFactor + enchantedStaminaModifier - cursedStaminaModifier;
            Debug.Log(
                $"EquipmentWrapper: Applied stat modifiers with scale factor {scaledFactor} to {equipmentData.equipmentName}"
            );
        }
    }
}
