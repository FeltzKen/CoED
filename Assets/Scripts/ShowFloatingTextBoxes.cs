using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class FloatingTextManager : MonoBehaviour
    {
        public static FloatingTextManager Instance { get; private set; }

        [Header("Floating Text Settings")]
        [SerializeField]
        private GameObject floatingTextPrefab;

        [SerializeField]
        private Transform canvasTransform;

        [SerializeField]
        private float textDuration = 2f;

        [SerializeField]
        private Vector3 offset = new Vector3(0, 1f, 0);

        private void Awake()
        {
            if (floatingTextPrefab == null || canvasTransform == null)
            {
                Debug.LogError(
                    "FloatingTextManager: Missing prefab or canvas transform reference. Please assign these in the inspector."
                );
            }
        }

        public void ShowFloatingText(string message, Vector3 position, Color color)
        {
            if (floatingTextPrefab == null || canvasTransform == null)
            {
                Debug.LogWarning(
                    "FloatingTextManager: Cannot display floating text because prefab or canvas transform is not assigned."
                );
                return;
            }

            GameObject floatingTextInstance = Instantiate(floatingTextPrefab, canvasTransform);
            Text floatingText = floatingTextInstance.GetComponent<Text>();
            if (floatingText != null)
            {
                floatingText.text = message;
                floatingText.color = color;
            }

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position + offset);
            floatingTextInstance.transform.position = screenPosition;

            Destroy(floatingTextInstance, textDuration);
        }
    }
}
