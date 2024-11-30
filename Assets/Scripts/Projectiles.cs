using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "GameItems/Projectile", order = 1)]
public class Projectiles : ScriptableObject
{
    public string projectileName; // Name of the projectile
    public float speed; // Speed of the projectile
    public int damage; // Damage dealt by the projectile
    public float range; // Maximum range before the projectile disappears
    public GameObject projectilePrefab; // Prefab for the projectile
    public ParticleSystem impactEffect; // Visual effect on impact
    public bool piercesTargets; // Can the projectile hit multiple targets?
    public bool isHoming; // Does the projectile home in on targets?
    public bool isEnemyProjectile; // Is the projectile fired by an enemy?
    public bool isPlayerProjectile; // Is the projectile fired by the player?
    public bool isExplosive; // Does the projectile explode
    public float explosionRadius; // Radius of the explosion
    public float explosionForce; // Force of the explosion
    public float explosionDamage; // Damage of the explosion



    [TextArea]
    public string description; // Description of the projectile
}
