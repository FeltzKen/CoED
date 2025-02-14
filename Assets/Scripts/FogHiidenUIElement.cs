using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class FogHiddenUIElement : MonoBehaviour
    {
        // Reference to the enemy's transform.
        // This can be set in the inspector or automatically detected.
        public Transform enemyTransform;

        // The floor number to check against.
        public int floorNumber = 1;

        // The threshold at which the cell is considered "visible."
        // Should match the value used in your fog shader (e.g., 0.1).
        public float visibilityThreshold = 0.1f;

        // Cache a reference to a CanvasGroup on this healthbar.
        // CanvasGroup allows you to easily control the alpha of all UI elements under this GameObject.
        private CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Add a CanvasGroup if one isn't attached.
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            // If enemyTransform hasn't been set, try using the parent transform.
            if (enemyTransform == null && transform.parent != null)
            {
                enemyTransform = transform.parent;
            }
            floorNumber = enemyTransform.GetComponent<_EnemyStats>().spawnFloorLevel;
        }

        void Update()
        {
            if (enemyTransform == null || FogOfWarManager.Instance == null)
                return;

            // Convert the enemy's world position to grid coordinates.
            Vector2Int cellPos = new Vector2Int(
                Mathf.RoundToInt(enemyTransform.position.x),
                Mathf.RoundToInt(enemyTransform.position.y)
            );

            // Get the fog coverage for that cell.
            float coverage = FogOfWarManager.Instance.GetFogCoverage(cellPos, floorNumber);

            // If the cell is visible (coverage < threshold), show the healthbar;
            // otherwise, hide it.
            canvasGroup.alpha = (coverage < visibilityThreshold) ? 1f : 0f;
        }
    }
}
