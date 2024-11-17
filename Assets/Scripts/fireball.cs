using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float lifespan = 2f;
    public GameObject explosionPrefab;
    public LayerMask collisionLayers;
    public Transform playerProjectileContainer; // Set this in the Inspector

    private int damage = 25;
    private Vector3 direction;
    private float spawnTime;

    public void Launch(Vector3 launchDirection)
    {
        direction = launchDirection.normalized;
        spawnTime = Time.time;

        // Set parent to playerProjectileContainer if available
        if (playerProjectileContainer != null)
        {
            transform.SetParent(playerProjectileContainer);
        }
    }

    void Update()
    {
        // Move fireball in its launch direction
        transform.position += direction * speed * Time.deltaTime;

        // Destroy after lifespan
        if (Time.time > spawnTime + lifespan)
        {
            Destroy(gameObject);
        }
    }

private void OnTriggerEnter2D(Collider2D other)
{
    // Ignore collisions with the player
    if (other.CompareTag("Player")) return;

    Debug.Log($"Player taking damage from fireball: {damage}");

    // Check if the collided object is in the specified collision layers
    if (((1 << other.gameObject.layer) & collisionLayers) != 0)
    {
        // Instantiate explosion effect as child of playerProjectileContainer
        if (explosionPrefab != null && playerProjectileContainer != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity, playerProjectileContainer);
        }

        // Check for enemy health component and apply damage
        EnemyStats enemyStats = other.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage);
        }

        // Destroy the fireball on collision
        Destroy(gameObject);
    }
}

}
}