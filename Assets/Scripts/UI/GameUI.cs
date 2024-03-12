using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    MainMenu mainMenuPanel;
    [SerializeField]
    GameObject sidePanel;
    [SerializeField]
    MoveSelectionScreen moveSelectionPanel;

    [SerializeField]
    bool openMainMenuOnStart;

    GameObject mainMenuGO;
    GameObject moveSelectionGO;

    void Start() {
        mainMenuGO = mainMenuPanel.gameObject;
        moveSelectionGO = moveSelectionPanel.gameObject;

        gameFlow.OnStateChanged += OnGameStateChanged;
        gameFlow.OnPlaneSpawned += OnPlaneSpawned;

        if (openMainMenuOnStart) {
            OpenMainMenu();
        }
    }

    public void OpenMainMenu() {
        TogglePanels(mainMenuGO);
    }

    public void OnBeginMoveSelection() {
        TogglePanels(moveSelectionGO);
    }

    public void OnMoveSelected(ManeuverData playerMove, ManeuverData opponentMove) {
        gameFlow.SetPlayerMove(playerMove, opponentMove);
        gameFlow.PlayManeuvers();

        TogglePanels(sidePanel);
    }

    void OnGameStateChanged(GameFlow.GameState state) {

    }

    void OnPlaneSpawned(GameObject planePrefab) {
        moveSelectionPanel.SetPlane(planePrefab.GetComponent<Plane>());
    }

    void TogglePanels(GameObject targetPanel) {
        TogglePanel(targetPanel, mainMenuGO);
        TogglePanel(targetPanel, sidePanel);
        TogglePanel(targetPanel, moveSelectionGO);
    }

    void TogglePanel(GameObject targetPanel, GameObject panel) {
        if (panel != null) {
            panel.SetActive(panel == targetPanel);
        }
    }
}
