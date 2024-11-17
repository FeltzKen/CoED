using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.z = transform.position.z;  // Keep the camera's Z position constant
            transform.position = newPosition;
        }
    }

    public void SetPlayerTarget(Transform player)
    {
        playerTransform = player;
    }
}
