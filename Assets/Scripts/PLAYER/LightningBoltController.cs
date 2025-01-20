using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class LightningBoltController : MonoBehaviour
    {
        [SerializeField]
        private Sprite topSegmentSprite;

        [SerializeField]
        private Sprite middleSegmentSprite;

        [SerializeField]
        private Sprite bottomSegmentSprite;

        [SerializeField]
        private GameObject segmentPrefab; // Prefab with SpriteRenderer

        [SerializeField]
        private Transform segmentsRoot; // Parent object for organizing segments

        [SerializeField]
        private LayerMask targetLayer; // For collision detection

        [SerializeField]
        private float segmentLength = 0.25f;
        private float movementSpeed;
        private Vector3 targetPosition;
        private float lifetimeTimer;

        private int segmentIndex = 0;
        private Coroutine lightningCoroutine;

        [Header("Dynamic Damage and Effects")]
        [SerializeField]
        private Dictionary<DamageType, float> damageTypes = new Dictionary<DamageType, float>
        {
            { DamageType.Lightning, 20f }, // Base lightning damage
        };

        [SerializeField]
        private List<StatusEffectType> inflictedEffects = new List<StatusEffectType>
        {
            StatusEffectType.Stun // Default to stun effect
            ,
        };

        public void CreateLightningBolt(Vector3 start, Vector3 end, float speed, float lifetime)
        {
            movementSpeed = speed;
            targetPosition = end;
            lifetimeTimer = Time.time + lifetime;

            Vector3 currentPosition = start;
            Vector3 direction = (end - start).normalized;

            if (lightningCoroutine == null)
            {
                lightningCoroutine = StartCoroutine(AnimateLightning(currentPosition, direction));
            }
        }

        private IEnumerator AnimateLightning(Vector3 start, Vector3 direction)
        {
            Vector3 currentPosition = start;

            while (Vector3.Distance(currentPosition, targetPosition) > segmentLength)
            {
                if (Time.time > lifetimeTimer)
                {
                    Destroy(gameObject);
                    yield break;
                }

                currentPosition += direction * (movementSpeed * Time.deltaTime);

                CheckForCollision(currentPosition);
                CreateCycledSegment(currentPosition);

                yield return null;
            }

            CreateCycledSegment(targetPosition);
            Destroy(gameObject);
        }

        private void CreateCycledSegment(Vector3 position)
        {
            GameObject segment = Instantiate(
                segmentPrefab,
                position,
                Quaternion.identity,
                segmentsRoot
            );
            SpriteRenderer spriteRenderer = segment.GetComponent<SpriteRenderer>();

            switch (segmentIndex)
            {
                case 0:
                    spriteRenderer.sprite = topSegmentSprite;
                    break;
                case 1:
                    spriteRenderer.sprite = middleSegmentSprite;
                    break;
                case 2:
                    spriteRenderer.sprite = bottomSegmentSprite;
                    break;
            }

            segmentIndex = (segmentIndex + 1) % 3;
        }

        /// <summary>
        /// Checks for collisions and applies dynamic damage and effects.
        /// </summary>
        private void CheckForCollision(Vector3 position)
        {
            float radius = 0.25f;
            Collider2D hit = Physics2D.OverlapCircle(position, radius, targetLayer);

            if (hit != null)
            {
                Debug.Log($"Lightning hit {hit.gameObject.name}!");
                HandleHit(hit);
            }
        }

        /// <summary>
        /// Applies dynamic damage and effects on hit.
        /// </summary>
        private void HandleHit(Collider2D hit)
        {
            // ✅ Package dynamic damage and effects
            DamageInfo damageInfo = new DamageInfo(damageTypes, inflictedEffects);

            if (hit.CompareTag("Enemy"))
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageInfo);
                    Debug.Log($"{hit.gameObject.name} took dynamic lightning damage!");
                }
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, segmentLength);
        }
    }
}
