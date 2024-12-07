using UnityEngine;
using TMPro;
using System.Collections; // If using TextMeshPro

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
        private float floatSpeed = 2f; // Speed at which the text floats up

        [SerializeField]
        private float floatStep = 0.1f; // Distance to move the text in each step

        [SerializeField]
        private float stepInterval = 0.1f; // Time interval between each step

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
            // Create a new GameObject for the floating text
            GameObject floatingTextInstance = new GameObject("FloatingText");

            // Set the parent transform
            floatingTextInstance.transform.SetParent(parentTransform);

            // Set the local position with the offset
            floatingTextInstance.transform.localPosition = offset;

            // Add a TextMeshProUGUI component
            TextMeshProUGUI textComponent = floatingTextInstance.AddComponent<TextMeshProUGUI>();
            textComponent.text = message;
            textComponent.color = color;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 0.4f; // Adjust the font size as needed

            // Start the coroutine to float the text upwards and destroy it after the duration
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

                // Move the text up by the specified step amount
                floatingTextInstance.transform.localPosition += Vector3.up * floatStep;

                // Wait for the specified interval before moving again
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