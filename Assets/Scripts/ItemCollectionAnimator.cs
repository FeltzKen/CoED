using System.Collections;
using CoED;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollectionAnimator : MonoBehaviour
{
    public static ItemCollectionAnimator Instance { get; private set; }

    [SerializeField]
    private Canvas uiCanvas; // Reference to the Canvas

    [SerializeField]
    private float animationDuration = 1.5f;

    [SerializeField]
    private AnimationCurve movementCurve;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimateItemCollection(Sprite icon, Vector3 startPosition, RectTransform targetPanel)
    {
        GameObject floatingIcon = new GameObject("FloatingIcon", typeof(Image));
        floatingIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        Image image = floatingIcon.GetComponent<Image>();
        image.sprite = icon;
        floatingIcon.transform.SetParent(uiCanvas.transform, false);
        floatingIcon.transform.position = startPosition;

        StartCoroutine(MoveToDynamicTarget(floatingIcon, targetPanel));
    }

    private IEnumerator MoveToDynamicTarget(GameObject floatingIcon, RectTransform targetPanel)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = floatingIcon.transform.position;
        Vector3 initialScale = floatingIcon.transform.localScale;
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 1); // Target scale for the shrinking effect
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            if (targetPanel != null)
            {
                // Fetch the target panel's world position
                Vector3 targetPosition = targetPanel.position;

                // Adjust animation progress with curve
                float t = movementCurve.Evaluate(elapsedTime / animationDuration);

                // Update floating icon's position
                floatingIcon.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

                // Update floating icon's scale
                floatingIcon.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            }
            else
            {
                break; // Stop the animation if target panel is null
            }

            yield return null;
        }

        Destroy(floatingIcon);
    }
}
