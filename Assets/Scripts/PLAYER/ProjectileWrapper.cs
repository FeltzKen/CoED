using UnityEngine;

namespace CoED
{
    public class ProjectileWrapper : MonoBehaviour
    {
        public Projectile BaseProjectile { get; private set; }
        public Sprite Icon => BaseProjectile.icon;
        public string ProjectileName => BaseProjectile.projectileName;
        public GameObject ProjectilePrefab => BaseProjectile.projectilePrefab;
        public int Damage { get; private set; }
        public float Lifetime { get; private set; }
        public float projectileRange { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public bool CanChase => BaseProjectile.canChase;

        [SerializeField]
        private StatusEffect statusEffect;

        public void Initialize(Projectile baseProjectile)
        {
            BaseProjectile = baseProjectile;
            Damage = baseProjectile.damage;
            Lifetime = baseProjectile.lifetime;
            CollisionRadius =
                baseProjectile.collisionRadius > 0 ? baseProjectile.collisionRadius : 0.5f;
            Speed = baseProjectile.speed;
            Cooldown = baseProjectile.cooldown;
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

        private void DetectCollision()
        {
            // Detects if the projectile overlaps with the player
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                CollisionRadius,
                LayerMask.GetMask("player")
            );
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Debug.Log($"Player hit by {ProjectileName} for {Damage} damage!");

                    PlayerStats.Instance.TakeDamage(Damage);

                    // Apply a random status effect
                    if (Random.value < 0.1f)
                    {
                        Debug.Log($"Player inflicted with {statusEffect.effectName}!");
                        StatusEffectManager.Instance.AddStatusEffect(hit.gameObject, statusEffect);
                    }

                    Destroy(gameObject);
                }
                else if (hit.CompareTag("Walls"))
                {
                    // Destroy the projectile if it hits a wall
                    Destroy(gameObject);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize the overlap area in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CollisionRadius);
        }
    }
}
