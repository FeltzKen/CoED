using System.Collections.Generic;

namespace CoED
{
    public struct DamageInfo
    {
        public Dictionary<DamageType, float> DamageAmounts; // Different damage types
        public List<StatusEffectType> InflictedStatusEffects; // Status effects to apply

        public DamageInfo(
            Dictionary<DamageType, float> damageAmounts,
            List<StatusEffectType> statusEffects,
            float statusEffectDuration = 15f
        )
        {
            DamageAmounts = damageAmounts;
            InflictedStatusEffects = statusEffects;
        }
    }
}
