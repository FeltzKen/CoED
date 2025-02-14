namespace CoED
{
    public enum DamageType
    {
        None,
        Physical,
        Fire,
        Ice,
        Poison,
        Lightning,
        Arcane,
        Bleed,
        Holy,
        Shadow,
        Heal,
        Nature,
    }

    public enum Stat
    {
        None,
        HP,
        MaxHP,
        Magic,
        MaxMagic,
        Stamina,
        MaxStamina,
        Attack,
        Defense,
        Speed,
        Intelligence,
        Dexterity,
        Shield,
        FireRate,
        CritChance,
        CritDamage,
        ProjectileRange,
        AttackRange,
        ElementalDamage,
        ChanceToInflict,
        StatusEffectDuration,

        // Enemies only
        PatrolSpeed,
        ChaseSpeed,
    }

    public enum FogStatus
    {
        Unexplored, // The cell has never been seen.
        Explored, // The cell was seen in the past, but is not currently visible.
        Visible, // The cell is in the player’s current vision.
    }
}
