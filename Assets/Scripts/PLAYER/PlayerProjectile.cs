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
        public float damage { get; set; }
        public List<StatusEffect> statusEffects;

        [SerializeField]
        public float collisionRadius { get; set; }
        private Vector3 targetPosition;
        private bool hasReachedTarget = false;
        private bool hasCollided = false;

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
                    // Optionally instantiate an impact effect here
                    // Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }

        private void OnCollide()
        {
            if (hasCollided)
                return; // Prevent multiple calls
            hasCollided = true;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collisionRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyStats enemy = hit.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);

                        foreach (var effectPrefab in statusEffects)
                        {
                            if (effectPrefab != null)
                            {
                                Debug.Log($"Applying {effectPrefab.effectType} to {hit.name}.");
                                StatusEffectManager.Instance.AddStatusEffect(
                                    hit.gameObject,
                                    effectPrefab
                                );
                            }
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
    }
}
