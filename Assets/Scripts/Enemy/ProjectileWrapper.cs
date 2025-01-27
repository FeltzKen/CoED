using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class ProjectileWrapper : MonoBehaviour
    {
        public Projectile BaseProjectile { get; private set; }
        public Sprite Icon => BaseProjectile.icon;
        public string ProjectileName => BaseProjectile.projectileName;
        public GameObject ProjectilePrefab => BaseProjectile.projectilePrefab;
        public float chanceToApplyStatusEffect => BaseProjectile.chanceToApplyStatusEffect;
        public float Damage { get; private set; }
        public float Lifetime { get; private set; }
        public float ProjectileRange { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public bool CanChase => BaseProjectile.canChase;

        [SerializeField]
        private List<StatusEffectType> statusEffectTypes = new List<StatusEffectType>(); // Multiple effects

        [SerializeField]
        private Dictionary<DamageType, float> damageTypes = new Dictionary<DamageType, float>(); // Dynamic damage types

        public void Initialize(
            Projectile baseProjectile,
            Dictionary<DamageType, float> customDamage = null,
            List<StatusEffect> customEffects = null
        )
        {
            BaseProjectile = baseProjectile;
            Damage = baseProjectile.damage;
            Lifetime = baseProjectile.lifetime;
            CollisionRadius =
                baseProjectile.collisionRadius > 0 ? baseProjectile.collisionRadius : 0.5f;
            Speed = baseProjectile.speed;
            Cooldown = baseProjectile.cooldown;

            // Initialize dynamic damage and effects
            damageTypes =
                customDamage
                ?? new Dictionary<DamageType, float> { { DamageType.Physical, Damage } };
            statusEffectTypes =
                customEffects?.ConvertAll(effect => effect.effectType)
                ?? new List<StatusEffectType>();
        }

        public void Launch(Vector2 direction)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * Speed;

                // Rotate to face the direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            }

            Destroy(gameObject, Lifetime);
        }

        private void Update()
        {
            DetectCollision();
        }

        /// <summary>
        /// Handles collision and applies dynamic damage and status effects.
        /// </summary>
        private void DetectCollision()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                CollisionRadius,
                LayerMask.GetMask("player")
            );
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Debug.Log($"Player hit by {ProjectileName}!");

                    // ✅ Package dynamic damage and effects
                    List<StatusEffectType> inflictedEffects = new List<StatusEffectType>();
                    foreach (var effect in statusEffectTypes)
                    {
                        inflictedEffects.Add(effect);
                    }

                    DamageInfo damageInfo = new DamageInfo(damageTypes, inflictedEffects);

                    // ✅ Apply damage and effects to the player
                    PlayerStats.Instance.TakeDamage(damageInfo, chanceToApplyStatusEffect);

                    Debug.Log(
                        $"Player took {Damage} damage and received {inflictedEffects.Count} effects."
                    );

                    Destroy(gameObject);
                }
                else if (hit.CompareTag("Walls"))
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CollisionRadius);
        }
    }
}
