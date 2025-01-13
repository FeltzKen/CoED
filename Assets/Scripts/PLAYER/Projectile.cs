using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Projectiles/Projectile")]
    public class Projectile : ScriptableObject
    {
        [Header("Basic Attributes")]
        public string projectileName;
        public GameObject projectilePrefab;
        public Sprite icon;
        public int damage;
        public float lifetime;
        public float collisionRadius;
        public float speed;
        public float cooldown;

        [Header("Chase Attributes")]
        public bool canChase;
    }
}
