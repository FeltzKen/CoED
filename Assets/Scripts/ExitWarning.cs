using UnityEngine;

public class ExitWarning : MonoBehaviour
{
    [TextArea]
    public string warningMessage = "Make sure you’re prepared before leaving!";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the player has a "Player" tag
        {
            DisplayWarning();
        }
    }

    private void DisplayWarning()
    {
        // Replace this with your preferred UI system or method to show the message
        // Debug.Log(warningMessage);
        // You could also invoke a UI popup, dialogue box, or other in-game message system here.
    }
}
