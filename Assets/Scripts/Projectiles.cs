using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "GameItems/Projectile", order = 2)]
public class Projectiles : ScriptableObject
{
    public string projectileName; // Name of the projectile
    public float speed; // Speed of the projectile
    public int damage; // Damage dealt by the projectile
    public float range; // Maximum range before the projectile disappears
    public GameObject prefab; // Prefab for the projectile
    public ParticleSystem impactEffect; // Visual effect on impact
    public bool piercesTargets; // Can the projectile hit multiple targets?

    [TextArea]
    public string description; // Description of the projectile
}
