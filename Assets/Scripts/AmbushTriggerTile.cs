using UnityEngine;
using YourGameNamespace;
namespace YourGameNamespace
{
    public class AmbushTriggerTile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered the trigger tile!");
                // Add functionality here (e.g., spawn an event, trap, etc.)
            }
        }
    }
}