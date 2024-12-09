using UnityEngine;

public class TemporarySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private void Start()
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("TemporarySpawn: Player instantiated at (0,0,0).");
        }
        else
        {
            Debug.LogError("TemporarySpawn: PlayerPrefab is not assigned.");
        }
    }
}