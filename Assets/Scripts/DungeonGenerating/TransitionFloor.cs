using UnityEngine;
using CoED;
using System.Collections;
//using System.Numerics;
namespace CoED
{
    public class TransitionFloor : MonoBehaviour
    {
    public int floorChangeValue; // +1 for down, -1 for up
    private Transform dungeonParent;
    private Transform player;
    public int StairID;
    private void Start()
    {
        // Find the dungeon parent dynamically
        dungeonParent = GameObject.Find("DungeonParent").transform;
        player = PlayerMovement.Instance.transform;
    }

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // Debug.Log("hello from stairs trigger");

        // Transition logic
        int currentFloor = PlayerStats.Instance.GetCurrentFloor();
        int newFloor = currentFloor + floorChangeValue;
        PlayerStats.Instance.currentFloor = newFloor;
        // Debug.Log($"Transitioning player from Floor {currentFloor} to Floor {newFloor}. Updated player's currentFloor value");

        // Find the target floor based on the new floor number
        Transform targetFloorParent = FindFloor(newFloor);
        if (targetFloorParent == null)
        {
            Debug.LogError($"No floor found for Floor {newFloor}");
            return;
        }

        // Find the corresponding stairs on the target floor and disable its collider
        foreach (Transform child in targetFloorParent)
        {
            if (child.name.Contains("stairsUp") && floorChangeValue < 0 || child.name.Contains("stairsDown") && floorChangeValue > 0)
            {
                Collider2D targetCollider = child.GetComponent<Collider2D>();
                if (targetCollider != null)
                {
                    targetCollider.enabled = false;
                    // Debug.Log($"Disabled collider for stairs on Floor {newFloor}.");
                }
            }
        }

        // Calculate the position offset between the current floor and the target floor
        Vector3 offset = targetFloorParent.position - transform.parent.position;

        // Update the player's position
        Vector3 newPosition = player.position + offset + new Vector3(-0.5f, -0.5f, 0); // Adjust if needed
        PlayerMovement.Instance.UpdateCurrentTilePosition(newPosition);

        // Debug.Log($"Player transitioned to Floor {newFloor}, Position: {player.position}");
    }
}

            private IEnumerator WaitForPlayerToMove()
            {
                Vector3 originalPosition = player.position;

                // Wait until the player has moved a certain distance away from the original position
                while (Vector3.Distance(player.position, originalPosition) < 1.5f)
                {
                    yield return null;
                }

                // Re-enable the collider once the player is far enough
                GetComponent<Collider2D>().enabled = true;
            }
        private Transform FindFloor(int floorNumber)
        {
            // Search through the dungeon parent for a floor with the matching floor number
            foreach (Transform floor in dungeonParent)
            {
                if (floor.name.Contains($"Floor_{floorNumber}"))
                {
                    // Debug.Log($"Floor found: {floor.name}");
                    return floor;
                }
            }

            Debug.LogWarning($"No floor with FloorNumber: {floorNumber} found under dungeon parent.");
            return null;
        }
    }
}
