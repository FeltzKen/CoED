using System.Collections;
using UnityEngine;

namespace CoED
{
    public class StatusEffect : MonoBehaviour
    {
        public StatusEffectType effectType;
        public float damagePerSecond;
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

            ApplyEffect();
        }

        private void ApplyEffect()
        {
            switch (effectType)
            {
                case StatusEffectType.Burn:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyBurnEffect));
                    break;
                case StatusEffectType.Freeze:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyFreezeEffect));
                    break;
                case StatusEffectType.Poison:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyPoisonEffect));
                    break;
                case StatusEffectType.Stun:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyStunEffect));
                    break;
                case StatusEffectType.Slow:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplySlowEffect));
                    break;
                case StatusEffectType.Regen:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyRegenEffect));
                    break;
                case StatusEffectType.Shield:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyShieldEffect));
                    break;
                case StatusEffectType.Invincible:
                    effectCoroutine = StartCoroutine(EffectCoroutine(ApplyInvincibleEffect));
                    break;
                default:
                    Debug.LogWarning($"StatusEffect: Unknown effect type {effectType}.");
                    Destroy(gameObject);
                    break;
            }
        }

        private IEnumerator EffectCoroutine(System.Action effectLogic)
        {
            float elapsed = 0f;
            while (!hasDuration || elapsed < duration)
            {
                effectLogic.Invoke();
                elapsed += Time.deltaTime;
                yield return null;
            }
            RemoveEffect();
        }

        private void ApplyBurnEffect()
        {
            if (playerStats != null)
            {
                playerStats.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
            }
            else if (enemyStats != null)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
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
                playerStats.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
            }
            else if (enemyStats != null)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
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
                playerStats.Heal(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
            }
            else if (enemyStats != null)
            {
                enemyStats.Heal(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
            }
        }

        private void ApplyShieldEffect()
        {
            if (playerStats != null)
            {
                //playerStats.AddShield(10); // Example logic for shield
            }
            else if (enemyStats != null)
            {
                //enemyStats.AddShield(10); // Example logic for shield
            }
        }

        private void ApplyInvincibleEffect()
        {
            if (playerStats != null)
            {
                //playerStats.SetInvincible(true);
            }
            else if (enemyStats != null)
            {
                //enemyStats.SetInvincible(true);
            }
        }

        private void RemoveEffect()
        {
            if (effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
            }

            if (effectType == StatusEffectType.Invincible)
            {
                if (playerStats != null)
                {
                    //playerStats.SetInvincible(false);
                }
                else if (enemyStats != null)
                {
                    //enemyStats.SetInvincible(false);
                }
            }

            Destroy(gameObject);
        }
    }
}
