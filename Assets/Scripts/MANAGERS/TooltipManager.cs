using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField]
    private Transform PlayerUITransform;

    [SerializeField]
    private GameObject tooltipInstance;
    private GameObject tooltipPanel;
    private TMP_Text tooltipText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Initialize()
    {
        tooltipPanel = Instantiate(tooltipInstance, PlayerUITransform);
        Debug.Log("TooltipManager: Tooltip panel instantiated.");
        tooltipText = tooltipPanel.GetComponentInChildren<TMP_Text>();
        Debug.Log("TooltipManager: Tooltip text component found.");
        tooltipPanel.SetActive(false); // Start with the tooltip hidden
    }

    public void ShowTooltip(string description, Vector3 position)
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            Debug.LogWarning("TooltipManager: Tooltip components are not initialized properly.");
            return;
        }
        tooltipText.text = description;

        // Convert screen position to world position for a world-space canvas
        RectTransform canvasRect = PlayerUITransform.GetComponent<RectTransform>();

        if (canvasRect != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                new Vector3(position.x, position.y - 50, 0),
                Camera.main, // Ensure this is your main world-space camera
                out localPoint
            );

            // Update tooltip panel position in the local space of the canvas
            tooltipPanel.GetComponent<RectTransform>().localPosition = localPoint;
        }
        else
        {
            Debug.LogWarning("TooltipManager: PlayerUITransform does not have a RectTransform.");
        }
        tooltipPanel.SetActive(true);
        Debug.Log("TooltipPanel active: " + tooltipPanel.activeSelf);
    }

    public void ShowFloorTooltip(string description, Vector3 worldPosition)
    {
        tooltipText.text = description;
        tooltipPanel.SetActive(true);

        // Convert world position to screen position for the tooltip.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        tooltipPanel.transform.position = screenPosition;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        Debug.Log("TooltipPanel active: " + tooltipPanel.activeSelf);

        if (tooltipPanel != null) { }
    }
}
