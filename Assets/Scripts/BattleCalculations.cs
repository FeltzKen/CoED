using UnityEngine;

namespace CoED
{
    public static class BattleCalculations
    {
        /// <summary>
        /// Calculates the chance to hit (in percent) based solely on the attacker’s and defender’s Dexterity.
        /// If both values are zero, we assume a default 50% chance.
        /// </summary>
        public static float CalculateHitChance(float attackerDexterity, float defenderDexterity)
        {
            if (attackerDexterity + defenderDexterity == 0)
                return 50f;
            return (attackerDexterity / (attackerDexterity + defenderDexterity)) * 100f;
        }

        /// <summary>
        /// Returns true if the attack lands.
        /// </summary>
        public static bool IsAttackSuccessful(float attackerDexterity, float defenderDexterity)
        {
            float hitChance = CalculateHitChance(attackerDexterity, defenderDexterity);
            return Random.value * 100f < hitChance;
        }

        /// <summary>
        /// Returns true if the defender dodges the attack (the inverse chance).
        /// </summary>
        public static bool IsDodged(float attackerDexterity, float defenderDexterity)
        {
            float hitChance = CalculateHitChance(attackerDexterity, defenderDexterity);
            return Random.value * 100f >= hitChance;
        }

        /// <summary>
        /// Determines whether the attack is critical based on the attacker's CritChance stat.
        /// </summary>
        public static bool IsCriticalHit(float critChance)
        {
            return Random.value < critChance;
        }

        /// <summary>
        /// Calculates damage from an attack. If a critical hit occurs, multiplies the base damage by the crit damage multiplier.
        /// </summary>
        public static float CalculateDamage(
            float baseAttack,
            float weaponPower,
            float targetDefense,
            bool isCritical,
            float critDamageMultiplier
        )
        {
            float damage = Mathf.Max(0, baseAttack + weaponPower - targetDefense);
            return isCritical ? damage * critDamageMultiplier : damage;
        }

        /// <summary>
        /// Determines whether a status effect should be applied based on the attacker's ChanceToInflict stat and Intelligence.
        /// </summary>
        public static bool ShouldApplyStatusEffect(
            float chanceToInflict,
            float attackerIntelligence
        )
        {
            float effectiveChance = chanceToInflict + (attackerIntelligence * 0.5f);
            return Random.value * 100f < effectiveChance;
        }
    }
}
