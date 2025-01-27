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
        RandomDebuff,
        StealHealth,
        Bleed,
        Curse,
        Blindness,
        Silence,

        DamageReflect, // Reflects a portion of incoming damage

        // Add more effects as needed
    }

    public enum ActiveWhileEquipped
    {
        ReviveOnce, // Triggers once to revive the player
    }

    public enum Resistances
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Darkness,
    }

    public enum Weaknesses
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Darkness,
    }

    public enum Immunities
    {
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy,
        Darkness,
    }
}
