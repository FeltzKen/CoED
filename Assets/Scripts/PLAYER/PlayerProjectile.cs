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
        public float damage { get; set; }

        [SerializeField]
        public float collisionRadius { get; set; } // Radius for collision detection
        private Vector3 targetPosition;
        private bool hasReachedTarget = false;

        public void SetTargetPosition(Vector3 target)
        {
            Debug.Log($"PlayerProjectile: Setting target position to {target}");
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
                Debug.Log($"PlayerProjectile: Moving towards {targetPosition} with step {step}");

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    hasReachedTarget = true;
                    Debug.Log("PlayerProjectile: Reached target position.");
                    // Optionally instantiate an impact effect here
                    // Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
                    Destroy(gameObject);
                }

                OnCollide();
            }
        }

        private void OnCollide()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collisionRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyStats enemy = hit.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        Debug.Log(
                            $"PlayerProjectile: Hit enemy {hit.name}, dealing {damage} damage."
                        );
                        enemy.TakeDamage(damage);
                    }
                    else
                    {
                        Debug.LogWarning(
                            "PlayerProjectile: EnemyStats component not found on collided object."
                        );
                    }
                    Destroy(gameObject);
                    return;
                }
                else if (hit.CompareTag("Obstacle"))
                {
                    Debug.Log("PlayerProjectile: Hit an obstacle.");
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
