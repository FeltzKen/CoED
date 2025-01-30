using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class ConsumableItem : IShopItem
    {
        [Header("Base Stats")]
        public string name;
        public string description;
        public Sprite icon;
        public bool hasDuration = true;
        public float duration;
        public float prefixDuration;
        public float suffixDuration;
        public float attackBoost = 0;
        public float defenseBoost = 0;
        public float speedBoost = 0;
        public float healthBoost = 0;
        public float magicBoost = 0;
        public float staminaBoost = 0;
        public float dexterityBoost = 0;
        public float intelligenceBoost = 0;
        public float critChanceBoost = 0;
        public bool canHaveAffixes = true;
        public int amountPerInterval = 0;
        public int price;
        public int suffixPriceIncrease;
        public float prefixPriceMultiplier = 1;

        [Header("Affixes")]
        public ConsumablePrefixData prefix;
        public ConsumableSuffixData suffix;

        [Header("Effects")]
        public List<StatusEffectType> addedEffects = new();
        public List<StatusEffectType> removedEffects = new();

        public string GetName() => name;

        public Sprite GetSprite() => icon;

        public void Initialize(
            string name,
            string description,
            Sprite icon,
            float duration,
            float attackBoost,
            float defenseBoost,
            float speedBoost,
            float healthBoost,
            float magicBoost,
            float staminaBoost,
            float dexterityBoost,
            float intelligenceBoost,
            float critChanceBoost,
            List<StatusEffectType> addedEffects,
            List<StatusEffectType> removedEffects,
            bool canHaveAffixes,
            int amountPerInterval,
            int price
        )
        {
            // apply prefix and suffix if not null.
            this.name = name;
            this.description = description;
            this.icon = icon;
            this.duration = duration;
            this.attackBoost = attackBoost;
            this.defenseBoost = defenseBoost;
            this.speedBoost = speedBoost;
            this.healthBoost = healthBoost;
            this.magicBoost = magicBoost;
            this.staminaBoost = staminaBoost;
            this.dexterityBoost = dexterityBoost;
            this.intelligenceBoost = intelligenceBoost;
            this.critChanceBoost = critChanceBoost;
            this.addedEffects = addedEffects;
            this.removedEffects = removedEffects;
            this.canHaveAffixes = canHaveAffixes;
            this.amountPerInterval = amountPerInterval;
            this.price = price;
        }

        public string GetDescription()
        {
            string desc = "";
            if (prefix != null)
                desc = prefix.prefixName + " " + name;
            if (suffix != null)
                desc = desc + " " + suffix.suffixName;
            desc = desc + "\n" + description;
            return desc;
        }

        public void Consume()
        {
            var playerStats = PlayerStats.Instance;
            if (playerStats != null)
            {
                if (healthBoost < 0)
                    playerStats.Heal(healthBoost);
                if (magicBoost > 0)
                    playerStats.RefillMagic(magicBoost);
                if (staminaBoost > 0)
                    playerStats.RefillStamina(staminaBoost);
                if (dexterityBoost > 0)
                    playerStats.StartCoroutine(playerStats.GainDexterity(dexterityBoost, duration));
                if (intelligenceBoost > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainIntelligence(intelligenceBoost, duration)
                    );
                if (critChanceBoost > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainCritChance(critChanceBoost / 100, duration)
                    );
                if (attackBoost > 0)
                    playerStats.StartCoroutine(playerStats.GainAttack(attackBoost, duration));
                if (defenseBoost > 0)
                    playerStats.StartCoroutine(playerStats.GainDefense(defenseBoost, duration));
                if (speedBoost > 0)
                    playerStats.StartCoroutine(playerStats.GainSpeed(speedBoost, duration));
            }

            foreach (var effect in addedEffects)
                StatusEffectManager.Instance.AddStatusEffect(playerStats.gameObject, effect);

            foreach (var effect in removedEffects)
                StatusEffectManager.Instance.RemoveSpecificEffect(playerStats.gameObject, effect);
        }

        public int GetPrice()
        {
            return (int)((price + suffixPriceIncrease) * prefixPriceMultiplier);
        }

        public void ApplyPrefix(ConsumablePrefixData prefix)
        {
            this.prefix = prefix;
            if (attackBoost < 0)
                attackBoost *= prefix.modifierAmount;
            if (defenseBoost < 0)
                defenseBoost *= prefix.modifierAmount;
            if (speedBoost < 0)
                speedBoost *= prefix.modifierAmount;
            if (healthBoost < 0)
                healthBoost *= prefix.modifierAmount;
            if (magicBoost < 0)
                magicBoost *= prefix.modifierAmount;
            if (staminaBoost < 0)
                staminaBoost *= prefix.modifierAmount;
            if (dexterityBoost < 0)
                dexterityBoost *= prefix.modifierAmount;
            if (intelligenceBoost < 0)
                intelligenceBoost *= prefix.modifierAmount;
            if (critChanceBoost < 0)
                critChanceBoost += prefix.modifierAmount;
            if (prefix.duration > 0)
                duration += prefix.duration;
            if (prefixPriceMultiplier > 0)
                prefixPriceMultiplier = prefix.priceMultiplier;
        }

        public void ApplySuffix(ConsumableSuffixData suffix)
        {
            this.suffix = suffix;
            attackBoost += suffix.attackBoost;
            defenseBoost += suffix.defenseBoost;
            speedBoost += suffix.speedBoost;
            healthBoost += suffix.healthBoost;
            magicBoost += suffix.magicBoost;
            staminaBoost += suffix.staminaBoost;
            dexterityBoost += suffix.dexterityBoost;
            intelligenceBoost += suffix.intelligenceBoost;
            critChanceBoost += suffix.critChanceBoost;
            suffixPriceIncrease = suffix.priceIncrease;
            duration += suffix.duration;

            foreach (var effect in suffix.addedEffects)
                addedEffects.Add(effect);

            foreach (var effect in suffix.removedEffects)
                removedEffects.Add(effect);
        }
    }
}
