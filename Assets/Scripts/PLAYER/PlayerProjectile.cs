using UnityEngine;

namespace CoED
{
    public class PlayerProjectile : MonoBehaviour
    {
        public Vector2 direction;
        public int damage;

        public float speed = 10f;
        public float lifetime = 5f;

        private Rigidbody2D rb;
        private Vector3 targetPosition;
        private bool hasReachedTarget = false;

        public void SetTargetPosition(Vector3 target)
        {
            Debug.Log($"PlayerProjectile: Setting target position to {target}");
            targetPosition = target;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
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
                    hasReachedTarget = true;
                    // Optionally instantiate an impact effect here
                    // Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyStats enemy = collision.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning(
                        "PlayerProjectile: EnemyStats component not found on collided object."
                    );
                }
                Destroy(gameObject);
            }
            else if (collision.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
            }
        }
    }

    public class Arrow : PlayerProjectile { }

    public class Spear : PlayerProjectile { }

    // Add more projectile types as needed
}
