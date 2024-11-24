using UnityEngine;
using CoED;

namespace CoED
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField]
        private float speed = 10f;

        [SerializeField]
        private int damage = 10;

        [SerializeField]
        private bool isEnemyProjectile = false;

        private Rigidbody2D rb;
        private Vector2 direction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Projectile: Rigidbody2D component missing.");
            }

            rb.freezeRotation = true;
        }

        public void Initialize(Vector3 targetPosition, int damageAmount, bool enemyProjectile)
        {
            damage = damageAmount;
            isEnemyProjectile = enemyProjectile;

            Vector2 startPosition = (Vector2)transform.position; // Convert to Vector2
            direction = ((Vector2)(targetPosition - (Vector3)startPosition)).normalized; // Ensure both are Vector2
            rb.linearVelocity = direction * speed; // Use rb.velocity instead of rb.linearVelocity for clarity

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnEnable()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Reset velocity
            }
        }

        private void OnDisable()
        {
            // Optionally reset any projectile state if needed
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isEnemyProjectile && collision.CompareTag("Player"))
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                    Debug.Log($"Projectile: Player hit for {damage} damage.");
                }
                else
                {
                    Debug.LogWarning("Projectile: PlayerStats component missing on Player.");
                }
                gameObject.SetActive(false);
            }
            else if (!isEnemyProjectile && collision.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(damage); // Fix variable name from enemyCombat to enemyAI
                    Debug.Log($"Projectile: Enemy hit for {damage} damage.");
                }
                else
                {
                    Debug.LogWarning("Projectile: EnemyAI component missing on Enemy.");
                }
                gameObject.SetActive(false);
            }
            else if (collision.CompareTag("Obstacle") || collision.CompareTag("Wall"))
            {
                Debug.Log("Projectile: Hit an obstacle and is disabled.");
                gameObject.SetActive(false);
            }
        }
    }
}
