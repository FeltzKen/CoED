using UnityEngine;

[CreateAssetMenu(fileName = "FloorConfig", menuName = "GameSettings/FloorConfig")]
public class FloorConfig : ScriptableObject
{
    public int floorNumber;
    public Vector2Int floorSize;
    public int maxEnemies;
    public int maxRooms;
}
