using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class UIPanelToggleManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField]
        private List<GameObject> panels; // List of all UI panels to manage

        [Header("Game State")]
        [SerializeField]
        private bool isGamePaused;

        private void Start()
        {
            isGamePaused = false;
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;
            isGamePaused = true;
        }

        private void UnpauseGame()
        {
            Time.timeScale = 1f;
            isGamePaused = false;
        }

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
                    bool isActive = !panel.activeSelf;
                    panel.SetActive(isActive); // Toggle the clicked panel

                    if (isActive)
                    {
                        PauseGame();
                    }
                    else
                    {
                        UnpauseGame();
                    }
                }
                else
                {
                    panel.SetActive(false); // Hide all other panels
                }
            }
        }
    }
}
