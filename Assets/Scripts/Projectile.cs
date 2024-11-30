using UnityEngine;
using CoED;
using System.Collections;
namespace CoED
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private int damage;
    private bool isEnemyProjectile;
    private ProjectileManager projectileManager;
    private float speed;

    public void Initialize(Vector3 targetPosition, int damage, bool isEnemyProjectile, ProjectileManager manager, float speed)
    {
        this.targetPosition = targetPosition;
        this.damage = damage;
        this.isEnemyProjectile = isEnemyProjectile;
        this.projectileManager = manager;
        this.speed = speed;

        // Start moving towards the target
        StartCoroutine(MoveTowardsTarget());
    }

    private IEnumerator MoveTowardsTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        // Handle collision or reaching the target
        OnHitTarget();
    }

    private void OnHitTarget()
    {
        // Implement damage logic here
        // For example, check for collision with player or enemy and apply damage

        // Return the projectile to the pool
        projectileManager.ReturnProjectileToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision with other objects
        OnHitTarget();
    }
}
}
