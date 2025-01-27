using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class UIPanelToggleManager : MonoBehaviour
    {
        public static UIPanelToggleManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField]
        private GameObject panel; // List of all UI panels to manage

        [Header("Game State")]
        [SerializeField]
        private bool isGamePaused;

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

        public void TogglePanel(GameObject panel)
        {
            if (panel == null)
            {
                Debug.LogWarning("UIPanelToggleManager: Panel to show is null.");
                return;
            }

            if (panel.activeSelf)
            {
                panel.SetActive(false);
                UnpauseGame();
                Debug.Log("UIPanelToggleManager: Panel closed.");
            }
            else
            {
                panel.SetActive(true);
                PauseGame();
                Debug.Log("UIPanelToggleManager: Panel opened.");
            }
        }
    }
}
