using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class UIPanelToggleManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField]
        private List<GameObject> panels; // List of all UI panels to manage

        /// <summary>
        /// Toggles the visibility of a panel and hides all others.
        /// </summary>
        /// <param name="panelToShow">The panel to show.</param>
        public void TogglePanel(GameObject panelToShow)
        {
            if (panelToShow == null)
            {
                Debug.LogWarning("UIPanelToggleManager: Panel to show is null.");
                return;
            }

            foreach (GameObject panel in panels)
            {
                if (panel == panelToShow)
                {
                    panel.SetActive(!panel.activeSelf); // Toggle the clicked panel
                }
                else
                {
                    panel.SetActive(false); // Hide all other panels
                }
            }
        }
    }
}
