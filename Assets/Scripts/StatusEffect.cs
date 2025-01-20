using UnityEngine;

namespace CoED
{
    public class StatusEffect : MonoBehaviour
    {
        [Header("General Effect Info")]
        public StatusEffectType effectType;
        public string effectName;
        public bool hasDuration = true;

        [Min(0.1f)]
        public float duration = 5f; // how long the effect lasts

        [Header("Periodic Tick Settings")]
        [Tooltip("For over-time effects (DoT, HoT).")]
        public bool hasPeriodicTick = false;

        [Min(0.1f)]
        public float tickInterval = 1f;
        private float tickTimer;

        // For doT or hoT amounts, you could place them here:
        public float tickDamageOrHeal = 5f;

        private GameObject targetEntity;
        private PlayerStats playerStats;
        private EnemyStats enemyStats;

        // Any immediate flags we set
        private bool wasInvincible = false;
        private bool addedDamageReflect = false;
        private bool addedShield = false;

        public void ApplyToEntity(GameObject entity)
        {
            targetEntity = entity;
            if (entity.CompareTag("Player"))
            {
                playerStats = entity.GetComponent<PlayerStats>();
            }
            else
            {
                enemyStats = entity.GetComponent<EnemyStats>();
            }

            // Immediate effect application:
            switch (effectType)
            {
                case StatusEffectType.Regen:
                    // We can do an immediate small heal if desired, or just let OverTime ticks handle it
                    break;

                case StatusEffectType.Shield:
                    // Example: add 10 shield
                    if (playerStats != null)
                    {
                        playerStats.AddShield(10);
                        addedShield = true;
                    }
                    else if (enemyStats != null)
                    {
                        enemyStats.AddShield(10);
                        addedShield = true;
                    }
                    break;

                case StatusEffectType.Invincible:
                    if (playerStats != null)
                    {
                        playerStats.SetInvincible(true);
                        wasInvincible = true;
                    }
                    else if (enemyStats != null)
                    {
                        enemyStats.SetInvincible(true);
                        wasInvincible = true;
                    }
                    break;

                case StatusEffectType.DamageReflect:
                    // Mark that the player or enemy has reflect.
                    // Typically you handle the reflection inside TakeDamage,
                    // checking if "DamageReflect" is active in their stats list or a bool.
                    if (playerStats != null)
                    {
                        // Just add to the player's "activeStatusEffects" if needed
                        if (
                            !playerStats.activeStatusEffects.Contains(
                                StatusEffectType.DamageReflect
                            )
                        )
                        {
                            playerStats.activeStatusEffects.Add(StatusEffectType.DamageReflect);
                            addedDamageReflect = true;
                        }
                    }
                    else if (enemyStats != null)
                    {
                        // For an enemy, we could do something similar
                        if (
                            !enemyStats.activeStatusEffects.Contains(StatusEffectType.DamageReflect)
                        )
                        {
                            enemyStats.activeStatusEffects.Add(StatusEffectType.DamageReflect);
                            addedDamageReflect = true;
                        }
                    }
                    break;

                // “ReviveOnce” gets handled in PlayerStats.HandleDeath
                // We do not need to do anything here except maybe track a bool
                case StatusEffectType.ReviveOnce:
                    // no immediate action; the logic is in PlayerStats.HandleDeath
                    break;

                // etc. for any other immediate changes
            }
        }

        private void Update()
        {
            if (!hasPeriodicTick)
                return;

            // If we do have a periodic tick (like Burn or Poison or Regen), we do it here:
            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;
                ApplyPeriodicEffect();
            }
        }

        /// <summary>
        /// Called each tick for DoT/HoT etc.
        /// </summary>
        private void ApplyPeriodicEffect()
        {
            if (targetEntity == null)
                return;

            switch (effectType)
            {
                case StatusEffectType.Burn:
                    // Deal “Fire” damage
                    DoDamage(DamageType.Fire, tickDamageOrHeal);
                    break;

                case StatusEffectType.Poison:
                    // Deal “Poison” damage
                    DoDamage(DamageType.Poison, tickDamageOrHeal);
                    break;

                case StatusEffectType.Regen:
                    // Heal
                    DoHeal(tickDamageOrHeal);
                    break;
                // You can add more if you want “Slow” to do something each tick, etc.
            }
        }

        private void DoDamage(DamageType type, float amount)
        {
            if (playerStats != null)
            {
                var dmgDict = new System.Collections.Generic.Dictionary<DamageType, float>
                {
                    { type, amount },
                };
                var dmgInfo = new DamageInfo(
                    dmgDict,
                    new System.Collections.Generic.List<StatusEffectType>()
                );
                playerStats.TakeDamage(dmgInfo);
            }
            else if (enemyStats != null)
            {
                var dmgDict = new System.Collections.Generic.Dictionary<DamageType, float>
                {
                    { type, amount },
                };
                var dmgInfo = new DamageInfo(
                    dmgDict,
                    new System.Collections.Generic.List<StatusEffectType>()
                );
                enemyStats.TakeDamage(dmgInfo);
            }
        }

        private void DoHeal(float amount)
        {
            if (playerStats != null)
            {
                playerStats.Heal(amount);
            }
            else if (enemyStats != null)
            {
                enemyStats.Heal(Mathf.RoundToInt(amount));
            }
        }

        private void OnEnable()
        {
            tickTimer = tickInterval;
        }

        /// <summary>
        /// Called by StatusEffectManager when this effect is removed OR destroyed on scene unload.
        /// We revert any persistent changes here (like invincibility or reflect).
        /// </summary>
        private void OnDestroy()
        {
            // Undo any immediate changes we made in ApplyToEntity
            if (wasInvincible)
            {
                if (playerStats != null)
                    playerStats.SetInvincible(false);
                else if (enemyStats != null)
                    enemyStats.SetInvincible(false);
            }
            if (addedShield)
            {
                // If we added 10 shield, we can remove it now
                if (playerStats != null)
                    playerStats.RemoveShield(10);
                else if (enemyStats != null)
                    enemyStats.RemoveShield(10);
            }
            if (addedDamageReflect)
            {
                if (playerStats != null)
                {
                    playerStats.activeStatusEffects.Remove(StatusEffectType.DamageReflect);
                }
                else if (enemyStats != null)
                {
                    enemyStats.activeStatusEffects.Remove(StatusEffectType.DamageReflect);
                }
            }
        }
    }
}
