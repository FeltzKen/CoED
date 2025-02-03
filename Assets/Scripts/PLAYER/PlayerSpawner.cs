using UnityEngine;

namespace CoED
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance { get; private set; }

        [Header("Player Settings")]
        [SerializeField]
        private GameObject playerPrefab;

        private GameObject currentPlayer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerSpawner: Duplicate instance destroyed.");
                return;
            }
        }

        public GameObject SpawnPlayer()
        {
            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
            Vector3 spawnPosition = spawnPointObj.transform.position;
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            // If a class was selected, update the sprite accordingly.
            if (GameManager.SelectedClass != null)
            {
                SpriteRenderer sr = currentPlayer.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = GameManager.SelectedClass.CharacterSprite;
                }
            }

            GameManager.Instance.RegisterPlayer(currentPlayer);
            return currentPlayer;
        }
    }
}
