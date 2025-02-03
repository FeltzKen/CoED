using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public interface IEntityStats
    {
        void ModifyStat(Stat stat, float value);
        void NullStat(Stat stat, float duration); // I want to start a coroutine in this method to nullify the stat for a certain duration.

        void AddActiveStatusEffect(StatusEffectType effect);
        void RemoveActiveStatusEffect(StatusEffectType effect);
        void RemoveWeakness(Weaknesses weaknesses);
        void AddWeakness(Weaknesses weaknesses);
        void AddImmunity(Immunities immunities);
        void RemoveImmunity(Immunities immunities);
        void AddResistance(Resistances resistances);
        void RemoveResistance(Resistances resistances);
        bool HasStatusEffect(StatusEffectType effect);
        public void TakeDamage(
            DamageInfo damageInfo,
            float statusChance = 0.05f,
            bool bypassInvincible = false
        );
        void Heal(float amount);
        void IsSilenced(bool state);
        void SetInvincible(bool state);
        void AddShield(float amount);
        void RemoveShield(float amount);
    }

    public interface IShopItem
    {
        string GetName();
        string GetDescription();
        Sprite GetSprite();
    }
}
