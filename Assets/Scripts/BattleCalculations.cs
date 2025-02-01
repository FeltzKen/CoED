using UnityEngine;

namespace CoED
{
    public static class BattleCalculations
    {
        private const float dexterityScaleFactor = 5f;

        /// <summary>
        /// Calculates success rate based on Dexterity using a logarithmic function.
        /// Used for actions like attack accuracy, dodge chance, trap disarming, etc.
        /// </summary>
        public static float CalculateDexteritySuccess(float dexterity)
        {
            return 100f * (1 - Mathf.Exp(-dexterity / dexterityScaleFactor));
        }

        /// <summary>
        /// Determines hit chance based on attacker's accuracy and defender's evasion.
        /// </summary>
        public static float CalculateHitChance(float attackerAccuracy, float defenderEvasion)
        {
            return attackerAccuracy / (float)(attackerAccuracy + defenderEvasion) * 100f;
        }

        /// <summary>
        /// Determines if an attack lands based on hit chance.
        /// </summary>
        public static bool IsAttackSuccessful(float attackerAccuracy, float defenderEvasion)
        {
            return Random.value * 100f < CalculateHitChance(attackerAccuracy, defenderEvasion);
        }

        /// <summary>
        /// Calculates the probability of a critical hit based on Dexterity.
        /// </summary>
        public static bool IsCriticalHit(float dexterity)
        {
            float critChance = Mathf.Clamp(dexterity * 1.5f, 0, 100);
            return Random.value * 100f < critChance;
        }

        /// <summary>
        /// Calculates standard damage with optional critical hit multiplier.
        /// </summary>
        public static float CalculateDamage(
            float baseAttack,
            float weaponPower,
            float targetDefense,
            bool isCritical
        )
        {
            float damage = Mathf.Max(0, baseAttack + weaponPower - targetDefense);
            return isCritical ? damage * 1.5f : damage;
        }

        /// <summary>
        /// Determines if an attack is dodged based on defender's Dexterity.
        /// </summary>
        public static bool IsDodged(float defenderDexterity)
        {
            return Random.value * 100f < CalculateDexteritySuccess(defenderDexterity);
        }

        /// <summary>
        /// Determines if a status effect applies based on effect chance and attacker's floatelligence.
        /// </summary>
        public static bool ApplyStatusEffect(float baseChance, float attackerfloatelligence)
        {
            return Random.value * 100f < baseChance + (attackerfloatelligence * 0.5f);
        }
    }
}
