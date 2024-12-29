using CoED;
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
        FloatingTextManager.Instance.ShowFloatingText(
            warningMessage,
            PlayerMovement.Instance.transform,
            Color.red
        );
    }
}
