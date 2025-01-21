using System.Collections;
using TMPro;
using UnityEngine;

namespace CoED
{
    public class FloatingTextManager : MonoBehaviour
    {
        public static FloatingTextManager Instance { get; private set; }

        [Header("Floating Text Settings")]
        [SerializeField]
        private float textDuration = 2f;

        [SerializeField]
        private Vector3 offset = new Vector3(0, -1f, 0);

        [SerializeField]
        private float floatStep = 0.1f;

        [SerializeField]
        private float stepInterval = 0.1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ShowFloatingText(string message, Transform parentTransform, Color color)
        {
            // Create floating text object
            GameObject floatingTextInstance = new GameObject("FloatingText");

            // Ensure it appears on the UI
            floatingTextInstance.transform.SetParent(parentTransform, false);
            floatingTextInstance.transform.localPosition = offset;
            floatingTextInstance.transform.SetAsLastSibling(); // Ensure it's on top of other UI elements

            // Add and configure TextMeshProUGUI component
            TextMeshProUGUI textComponent = floatingTextInstance.AddComponent<TextMeshProUGUI>();
            textComponent.text = message;
            textComponent.color = color;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 0.5f; // size = 0.5f
            textComponent.raycastTarget = false; // Prevent blocking other UI interactions

            // Start animation and destroy after completion
            StartCoroutine(FloatAndDestroy(floatingTextInstance));
        }

        private IEnumerator FloatAndDestroy(GameObject floatingTextInstance)
        {
            float elapsedTime = 0f;

            while (elapsedTime < textDuration)
            {
                if (floatingTextInstance == null)
                {
                    yield break;
                }

                floatingTextInstance.transform.localPosition += Vector3.up * floatStep;

                yield return new WaitForSeconds(stepInterval);

                elapsedTime += stepInterval;
            }

            if (floatingTextInstance != null)
            {
                Destroy(floatingTextInstance);
            }
        }
    }
}
