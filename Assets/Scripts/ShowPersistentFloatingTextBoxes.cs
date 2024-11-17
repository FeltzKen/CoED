using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PersistentTextManager : MonoBehaviour
    {
        [Header("Persistent Text Settings")]
        [SerializeField]
        private GameObject persistentTextPrefab;

        [SerializeField]
        private Transform canvasTransform;

        [SerializeField]
        private Vector3 offset = new Vector3(0, 1f, 0);

        private void Awake()
        {
            if (persistentTextPrefab == null || canvasTransform == null)
            {
                Debug.LogError(
                    "PersistentTextManager: Missing prefab or canvas transform reference. Please assign these in the inspector."
                );
            }
        }

        public void ShowPersistentText(string message, Transform target, Color color)
        {
            if (persistentTextPrefab == null || canvasTransform == null)
            {
                Debug.LogWarning(
                    "PersistentTextManager: Cannot display persistent text because prefab or canvas transform is not assigned."
                );
                return;
            }

            GameObject persistentTextInstance = Instantiate(persistentTextPrefab, canvasTransform);
            Text persistentText = persistentTextInstance.GetComponent<Text>();
            if (persistentText != null)
            {
                persistentText.text = message;
                persistentText.color = color;
            }

            PersistentTextFollower follower =
                persistentTextInstance.AddComponent<PersistentTextFollower>();
            follower.Initialize(target, offset);
        }
    }

    public class PersistentTextFollower : MonoBehaviour
    {
        private Transform target;
        private Vector3 offset;

        public void Initialize(Transform target, Vector3 offset)
        {
            this.target = target;
            this.offset = offset;
        }

        private void Update()
        {
            if (target != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);
                transform.position = screenPosition;
            }
        }
    }
}
