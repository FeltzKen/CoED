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
        DamageReflect, // Reflects a portion of incoming damage
        DamageAbsorb, // Absorbs a portion of incoming damage
        DamageReduction, // Reduces a portion of incoming damage
        DamageIncrease, // Increases a portion of outgoing damage
        AttackSpeedIncrease,
        MovementSpeedIncrease,
        EvasionIncrease,
        DefenseIncrease,
        AccuracyIncrease,
        #endregion
        // Add more effects as needed
    }

    public enum ActiveWhileEquipped
    {
        ReviveOnce, // Triggers once to revive the player
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
    }
}
