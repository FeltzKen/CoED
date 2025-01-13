using System.Collections;
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

        private bool isAnimating = false;

        [SerializeField]
        private float segmentLength = 0.25f;
        private float movementSpeed;
        private Vector3 targetPosition;
        private float lifetimeTimer;

        private int segmentIndex = 0; // To track the current segment type
        private Coroutine lightningCoroutine;

        [SerializeField]
        private StatusEffect stunEffectPrefab; // Reference the stun effect prefab directly

        public void CreateLightningBolt(
            Vector3 start,
            Vector3 end,
            int damage,
            float speed,
            float lifetime
        )
        {
            movementSpeed = speed;
            targetPosition = end;
            lifetimeTimer = Time.time + lifetime;

            Vector3 currentPosition = start;
            Vector3 direction = (end - start).normalized;

            // Start animating the lightning
            if (!isAnimating)
            {
                lightningCoroutine = StartCoroutine(
                    AnimateLightning(currentPosition, direction, damage)
                );
            }
        }

        private IEnumerator AnimateLightning(Vector3 start, Vector3 direction, int damage)
        {
            isAnimating = true;
            Vector3 currentPosition = start;

            // Start cycling through segments
            while (Vector3.Distance(currentPosition, targetPosition) > segmentLength)
            {
                if (Time.time > lifetimeTimer)
                {
                    Destroy(gameObject); // Destroy the entire LightningBoltController
                    yield break;
                }

                currentPosition += direction * (movementSpeed * Time.deltaTime);

                CheckForCollision(currentPosition, damage);
                CreateCycledSegment(currentPosition);

                yield return null; // Wait for the next frame
            }

            // Create the final segment to complete the lightning bolt
            CreateCycledSegment(targetPosition);

            isAnimating = false;
            Destroy(gameObject); // Destroy the entire LightningBoltController
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

            // Cycle through the segment sprites
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

            // Cycle to the next segment type
            segmentIndex = (segmentIndex + 1) % 3;
        }

        private void CheckForCollision(Vector3 position, int damage)
        {
            float radius = 0.25f; // Adjust the radius to fit the size of your collider
            Collider2D hit = Physics2D.OverlapCircle(position, radius, targetLayer);

            if (hit != null)
            {
                Debug.Log($"Hit {hit.gameObject.name}!");
                HandleHit(hit, damage);
            }
        }

        private void HandleHit(Collider2D hit, int damage)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                    // Apply stun effect
                    if (stunEffectPrefab != null)
                    {
                        StatusEffectManager.Instance.AddStatusEffect(
                            hit.gameObject,
                            stunEffectPrefab
                        );
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
