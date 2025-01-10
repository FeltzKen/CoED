using System.Collections;
using UnityEngine;

namespace CoED
{
    public class StatusEffect : MonoBehaviour
    {
        public StatusEffectType effectType;
        public float interval;
        public float amountPerInterval;
        public float duration;
        public float speedModifier;
        public string effectName;
        public Sprite icon;
        public bool hasDuration = true;

        private Coroutine effectCoroutine;
        private PlayerStats playerStats;
        private EnemyStats enemyStats;

        public void ApplyToEntity(GameObject entity)
        {
            // Determine if the target is a player or an enemy
            if (entity.CompareTag("Player"))
            {
                playerStats = entity.GetComponent<PlayerStats>();
                if (playerStats == null)
                {
                    Debug.LogWarning($"StatusEffect: PlayerStats not found on {entity.name}.");
                    Destroy(gameObject);
                    return;
                }
            }
            else if (entity.CompareTag("Enemy"))
            {
                enemyStats = entity.GetComponent<EnemyStats>();
                if (enemyStats == null)
                {
                    Debug.LogWarning($"StatusEffect: EnemyStats not found on {entity.name}.");
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"StatusEffect: Unknown target type for {entity.name}.");
                Destroy(gameObject);
                return;
            }

            ApplyEffect(entity);
        }

        private void ApplyEffect(GameObject entity)
        {
            switch (effectType)
            {
                case StatusEffectType.Burn:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyBurnEffect));
                    break;
                case StatusEffectType.Freeze:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyFreezeEffect));
                    break;
                case StatusEffectType.Poison:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyPoisonEffect));
                    break;
                case StatusEffectType.Stun:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyStunEffect));
                    break;
                case StatusEffectType.Slow:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplySlowEffect));
                    break;
                case StatusEffectType.Regen:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyRegenEffect));
                    break;
                case StatusEffectType.Shield:
                    effectCoroutine = StartCoroutine(EffectCoroutine(entity, ApplyShieldEffect));
                    break;
                case StatusEffectType.Invincible:
                    effectCoroutine = StartCoroutine(
                        EffectCoroutine(entity, ApplyInvincibleEffect)
                    );
                    break;
                default:
                    Debug.LogWarning($"StatusEffect: Unknown effect type {effectType}.");
                    Destroy(gameObject);
                    break;
            }
        }

        private IEnumerator EffectCoroutine(GameObject entity, System.Action effectLogic)
        {
            float elapsed = 0f;
            float intervalTimer = 0f;

            while (!hasDuration || elapsed < duration)
            {
                elapsed += Time.deltaTime;
                intervalTimer += Time.deltaTime;

                if (intervalTimer >= interval)
                {
                    effectLogic.Invoke();
                    intervalTimer = 0f; // Reset interval timer
                }

                yield return null;
            }
            StatusEffectManager.Instance.RemoveEffect(entity, this);
        }

        private void ApplyBurnEffect()
        {
            if (playerStats != null)
            {
                playerStats.TakeDamage(Mathf.RoundToInt(amountPerInterval), true);
            }
            else if (enemyStats != null)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(amountPerInterval), true);
            }
        }

        private void ApplyFreezeEffect()
        {
            if (playerStats != null)
            {
                playerStats.CurrentSpeed *= speedModifier;
            }
            else if (enemyStats != null)
            {
                enemyStats.CurrentSpeed *= speedModifier;
            }
        }

        private void ApplyPoisonEffect()
        {
            if (playerStats != null)
            {
                playerStats.TakeDamage(Mathf.RoundToInt(amountPerInterval), true);
            }
            else if (enemyStats != null)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(amountPerInterval), true);
            }
        }

        private void ApplyStunEffect()
        {
            if (playerStats != null)
            {
                playerStats.CurrentSpeed = 0;
            }
            else if (enemyStats != null)
            {
                enemyStats.CurrentSpeed = 0;
            }
        }

        private void ApplySlowEffect()
        {
            if (playerStats != null)
            {
                playerStats.CurrentSpeed *= speedModifier;
            }
            else if (enemyStats != null)
            {
                enemyStats.CurrentSpeed *= speedModifier;
            }
        }

        private void ApplyRegenEffect()
        {
            if (playerStats != null)
            {
                playerStats.Heal(Mathf.RoundToInt(amountPerInterval));
            }
            else if (enemyStats != null)
            {
                enemyStats.Heal(Mathf.RoundToInt(amountPerInterval));
            }
        }

        private void ApplyShieldEffect()
        {
            if (playerStats != null)
            {
                playerStats.AddShield(amountPerInterval);
            }
            else if (enemyStats != null)
            {
                enemyStats.AddShield(amountPerInterval);
            }
        }

        private void ApplyInvincibleEffect()
        {
            if (playerStats != null)
            {
                playerStats.SetInvincible(true);
            }
            else if (enemyStats != null)
            {
                enemyStats.SetInvincible(true);
            }
        }
    }
}
