using UnityEngine;

namespace CoED
{
    public class FogOfWar : MonoBehaviour
    {
        public static FogOfWar Instance { get; private set; }

        [SerializeField]
        private float revealRadius = 5f;

        [SerializeField]
        private LayerMask fogLayer;
        private Transform playerTransform;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("FogOfWar instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            playerTransform = PlayerStats.Instance?.transform;

            if (playerTransform == null)
            {
                Debug.LogError(
                    "FogOfWar: Player not found. Ensure Player script is properly set up."
                );
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            UpdateFogOfWar();
        }

        private void UpdateFogOfWar()
        {
            Collider2D[] fogTiles = Physics2D.OverlapCircleAll(
                playerTransform.position,
                revealRadius,
                fogLayer
            );

            foreach (var fogTile in fogTiles)
            {
                fogTile.gameObject.SetActive(false);
                // Debug.Log("FogOfWar: Revealed fog tile at " + fogTile.transform.position);
            }
        }
    }
}
