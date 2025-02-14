namespace CoED
{
    public enum StatusEffectType
    {
        None,
        RandomDebuff,
        RandomBuff,
        #region debuffs
        Burn,
        Slow,
        Stun,
        Freeze,
        Paralyze,
        Poison,
        PoisonAura,
        Invincible,
        Bleed,
        Curse,
        Blindness,
        Silence,
        Fear,
        Confusion,
        Sleep,
        Petrify,
        Root,
        Berserk,
        #endregion

        #region buffs
        Regen,
        StealHealth,
        Charm,
        Shield,
        DamageReflect,
        DamageAbsorb,
        DamageReduction,
        DamageIncrease,
        AttackSpeedIncrease,
        MovementSpeedIncrease,
        EvasionIncrease,
        DefenseIncrease,
        AccuracyIncrease,
        #endregion

        // Only available from equipment or consumables
        ReviveOnce,
    }

    public enum OneTimeConsumableEffect
    {
        ReviveOnce,
    }

    public enum Resistances
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Arcane,
        Nature,
        Earth,
    }

    public enum Weaknesses
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Arcane,
        Nature,
        Earth,
    }

    public enum Immunities
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Arcane,
        Nature,
        Earth,
    }
}
