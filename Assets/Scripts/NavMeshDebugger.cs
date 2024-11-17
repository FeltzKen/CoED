using UnityEngine;
using UnityEngine.AI;
using YourGameNamespace;

namespace YourGameNamespace
{
public class NavMeshDebugger : MonoBehaviour
{
    public Vector3 areaCenter = Vector3.zero;
    public float areaSize = 100f;
    public float sampleRadius = 2f;

    [ContextMenu("Run NavMesh Hit Test")]
    public void TestNavMeshHits()
    {
        int validHitCount = 0;
        areaCenter = new Vector3(10, 9, 0);
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPoint = areaCenter + new Vector3(
                Random.Range(-areaSize / 2, areaSize / 2),
                0,
                Random.Range(-areaSize / 2, areaSize / 2)
            );

            NavMeshHit hit;
            bool isOnNavMesh = NavMesh.SamplePosition(randomPoint, out hit, sampleRadius, NavMesh.AllAreas);

            if (isOnNavMesh)
            {
                validHitCount++;
                Debug.Log($"Valid NavMesh position at {hit.position} (original point: {randomPoint})");
            }
            else
            {
                Debug.LogWarning($"No NavMesh found near {randomPoint} within radius {sampleRadius}");
            }
        }

        Debug.Log($"Total valid NavMesh hits: {validHitCount} out of 100");
    }
}
}