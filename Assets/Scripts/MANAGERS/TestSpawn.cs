using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("TestSpawn: Start called.");
        
        if (CoED.PlayerSpawner.Instance != null)
        {
            Debug.Log("TestSpawn: PlayerSpawner Instance found.");
            CoED.PlayerSpawner.Instance.SpawnPlayer();
        }
        else
        {
            Debug.LogError("TestSpawn: PlayerSpawner instance is null.");
        }
    }
}