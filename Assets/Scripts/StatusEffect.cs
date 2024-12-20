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
        public string effectName { get; set; }
        private EnemyStats enemyStats;
        private Coroutine effectCoroutine;
        public Sprite Icon { get; private set; }

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                ApplyEffect();
            }
            else
            {
                Debug.LogWarning("StatusEffect: EnemyStats component not found.");
                Destroy(gameObject);
            }
        }

        private void ApplyEffect()
        {
            switch (effectType)
            {
                case StatusEffectType.Burn:
                    effectCoroutine = StartCoroutine(BurnEffect());
                    break;
                case StatusEffectType.Freeze:
                    effectCoroutine = StartCoroutine(FreezeEffect());
                    break;
                // Handle additional effects here
                default:
                    Debug.LogWarning($"StatusEffect: Unknown effect type {effectType}.");
                    Destroy(gameObject);
                    break;
            }
        }

        private IEnumerator BurnEffect()
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
                elapsed += Time.deltaTime;
                yield return null;
            }
            RemoveEffect();
        }

        private IEnumerator FreezeEffect()
        {
            // Example: Reduce enemy speed or stop movement
            enemyStats.CurrentSpeed = 0;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            enemyStats.ResetSpeed(); // Assume EnemyStats has a method to reset speed
            RemoveEffect();
        }

        private void RemoveEffect()
        {
            StopCoroutine(effectCoroutine);
            Destroy(gameObject);
        }
    }
}
