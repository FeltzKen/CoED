using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class ProjectileManager : MonoBehaviour
    {
        public static ProjectileManager Instance { get; private set; }

        [Header("Projectile Settings")]
        [SerializeField]
        private GameObject projectilePrefab;

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
            if (projectilePrefab == null)
            {
                Debug.LogError("ProjectileManager: Projectile Prefab is not assigned.");
                return;
            }

            if (projectileParent == null)
            {
                projectileParent = new GameObject("Projectiles").transform;
                projectileParent.parent = transform;
                Debug.Log("ProjectileManager: Created default Projectile Parent.");
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileParent);
                projectile.SetActive(false);
                projectilePool.Enqueue(projectile);
            }
        }

        public void LaunchProjectile(
            Vector3 startPosition,
            Vector3 targetPosition,
            int damage,
            bool isEnemyProjectile
        )
        {
            if (projectilePool.Count == 0)
            {
                Debug.LogWarning(
                    "ProjectileManager: No available projectiles in the pool. Consider increasing the pool size."
                );
                return;
            }

            GameObject projectileObj = projectilePool.Dequeue();
            projectileObj.transform.position = startPosition;
            projectileObj.transform.rotation = Quaternion.identity;
            projectileObj.SetActive(true);

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(targetPosition, damage, isEnemyProjectile);
                Debug.Log(
                    $"ProjectileManager: Launched projectile towards {targetPosition} with damage {damage}."
                );
            }
            else
            {
                Debug.LogError(
                    "ProjectileManager: Projectile Prefab does not contain a Projectile component."
                );
                projectileObj.SetActive(false);
                projectilePool.Enqueue(projectileObj);
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
    }
}
