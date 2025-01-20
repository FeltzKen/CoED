namespace CoED
{
    public enum StatusEffectType
    {
        // None,
        Burn,
        Slow,
        Stun,
        Regen,
        Freeze,
        Poison,
        Shadow,
        Holy,
        Shield,
        PoisonAura,
        Invincible,
        RandomBuff,
        RandomDebuff,
        DrainHealth,
        RandomUnequip,
        Rebirth,
        StatDecay,

        ReviveOnce, // Triggers once to revive the player
        DamageReflect, // Reflects a portion of incoming damage

        // Add more effects as needed
    }

    public enum Resistances
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
    }

    public enum Weaknesses
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
    }

    public enum Immunities
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
    }
}
