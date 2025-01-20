using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class PlayerProjectile : MonoBehaviour
    {
        [SerializeField]
        public Vector2 direction { get; set; }

        [SerializeField]
        public float speed { get; set; } = 2f;

        [SerializeField]
        public float lifetime { get; set; } = 5f;

        [SerializeField]
        public float collisionRadius { get; set; } = 0.5f;

        private Vector3 targetPosition;
        private bool hasReachedTarget = false;
        private bool hasCollided = false;

        [Header("Dynamic Damage and Status Effects")]
        [SerializeField]
        private Dictionary<DamageType, float> damageTypes = new Dictionary<DamageType, float>();

        [Header("Dynamic Damage and Status Effects")]
        private DamageInfo damageInfo; // ✅ Store the full DamageInfo object

        /// <summary>
        /// Initializes the projectile with custom damage and status effects.
        /// </summary>
        public void Initialize(DamageInfo info, Vector2 direction)
        {
            damageInfo = info; // ✅ Store the dynamic damage and effects
            this.direction = direction;
        }

        public void SetTargetPosition(Vector3 target)
        {
            targetPosition = target;
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (!hasReachedTarget)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    OnCollide();
                    hasReachedTarget = true;
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// Handles collision with enemies and applies dynamic damage and effects.
        /// </summary>
        private void OnCollide()
        {
            if (hasCollided)
                return;
            hasCollided = true;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collisionRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyStats enemy = hit.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        // Apply dynamic damage and effects
                        enemy.TakeDamage(damageInfo);

                        foreach (
                            var effect in GetComponent<PlayerSpellWrapper>().InflictedStatusEffectTypes
                        )
                        {
                            StatusEffectManager.Instance.AddStatusEffect(enemy.gameObject, effect);
                        }
                    }
                    Destroy(gameObject);
                    return;
                }
                else if (hit.CompareTag("Wall"))
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }
    }
}
