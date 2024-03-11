using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    GameObject mainMenuContainer;
    [SerializeField]
    GameObject mainMenuPanel;
    [SerializeField]
    GameObject singlePlayerPanel;
    [SerializeField]
    GameObject multiPlayerPanel;
    [SerializeField]
    GameObject optionsPanel;

    public void OpenMainMenu() {
        TogglePanels(null);
        mainMenuContainer.SetActive(true);
        mainMenuPanel.SetActive(true);
    }

    public void CloseMainMenu() {
        mainMenuContainer.SetActive(false);
    }

    public void OpenSinglePlayerMenu() {
        TogglePanels(singlePlayerPanel);
    }

    public void OpenMultiPlayerMenu() {
        TogglePanels(multiPlayerPanel);
    }

    public void OpenOptionsMenu() {
        TogglePanels(optionsPanel);
    }

    public void Quit() {
        Application.Quit();
    }

    void TogglePanels(GameObject targetPanel) {
        TogglePanel(targetPanel, singlePlayerPanel);
        TogglePanel(targetPanel, multiPlayerPanel);
        TogglePanel(targetPanel, optionsPanel);

        if (targetPanel != null) {
            mainMenuPanel.SetActive(false);
        }
    }

    void TogglePanel(GameObject targetPanel, GameObject panel) {
        if (panel != null) {
            panel.SetActive(panel == targetPanel);
        }
    }
}
