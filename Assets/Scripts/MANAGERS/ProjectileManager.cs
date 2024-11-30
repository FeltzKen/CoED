using System.Collections.Generic;
using UnityEngine;
using CoED;

namespace CoED
{
    public class ProjectileManager : MonoBehaviour
    {
        public static ProjectileManager Instance { get; private set; }

        [Header("Projectile Settings")]
        [SerializeField]
        private Projectiles projectiles;

        [SerializeField]
        private Transform projectileParent;

        [SerializeField]
        private int poolSize = 20;

        private Queue<GameObject> projectilePool = new Queue<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning(
                    "ProjectileManager instance already exists. Destroying duplicate."
                );
                return;
            }

            InitializeProjectilePool();
        }

        private void InitializeProjectilePool()
        {
            if (projectiles == null)
            {
                Debug.LogError("ProjectileManager: Projectile Prefab is not assigned.");
                return;
            }

            if (projectileParent == null)
            {
                projectileParent = new GameObject("Projectiles").transform;
                projectileParent.parent = transform;
                // Debug.Log("ProjectileManager: Created default Projectile Parent.");
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectile = Instantiate(projectiles.projectilePrefab, projectileParent);
                projectile.SetActive(false);
                projectilePool.Enqueue(projectile);
            }
        }

           public void LaunchProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        if (projectilePool.Count == 0)
        {
            Debug.LogWarning("ProjectileManager: No available projectiles in the pool. Consider increasing the pool size.");
            return;
        }
        Debug.Log("ProjectileManager: Launching projectile.");
        GameObject projectileObj = projectilePool.Dequeue();
        projectileObj.transform.position = startPosition;
        projectileObj.transform.rotation = Quaternion.identity;
        projectileObj.SetActive(true);

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(targetPosition, projectiles.damage, projectiles.isEnemyProjectile, this, projectiles.speed);
        }
    }

        public void ReturnToPool(GameObject projectile)
        {
            projectile.SetActive(false);
            projectilePool.Enqueue(projectile);
        }

        public GameObject GetPooledProjectile()
        {
            if (projectilePool.Count > 0)
            {
                return projectilePool.Dequeue();
            }
            else
            {
                Debug.LogWarning("ProjectileManager: No available projectiles in the pool.");
                return null;
            }
        }
        public void ReturnProjectileToPool(GameObject projectile)
        {
            projectile.SetActive(false);
            projectilePool.Enqueue(projectile);
        }
    }
}
