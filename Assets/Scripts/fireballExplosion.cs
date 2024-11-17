using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float duration = 0.5f; // Duration of the explosion animation
    private float timer = 0f;
    private Vector3 initialScale;
    private Vector3 finalScale;
    private Vector3 initialPosition;

    void Start()
    {
        initialScale = Vector3.one; // Start at scale (1,1,1)
        finalScale = new Vector3(3f, 3f, 1f); // Target scale (3,3,1)
        initialPosition = transform.position; // Store initial position for centering
    }

    void Update()
    {
        // Increase timer and calculate progress as a percentage of the duration
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration); // Normalizes between 0 and 1

        // Smoothly scale the explosion from initialScale to finalScale
        Vector3 currentScale = Vector3.Lerp(initialScale, finalScale, progress);
        transform.localScale = currentScale;

        // Adjust position to keep the explosion centered as it scales
        transform.position = initialPosition;

        // Destroy the explosion object after animation completes
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
