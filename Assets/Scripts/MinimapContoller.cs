using UnityEngine;
namespace CoED{
public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance {get; private set;}
    public Camera minimapCamera;
    public Transform floorCenter; // Assign the center of each floor in your dungeon.

    public void UpdateMinimapPosition(Vector3 newCenter)
    {
        minimapCamera.transform.position = new Vector3(newCenter.x, newCenter.y, minimapCamera.transform.position.z);
    }
}
}