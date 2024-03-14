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
    Gauges gaugesPanel;

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
        gaugesPanel.Show(false);
    }

    public void OpenSidePanel() {
        TogglePanels(sidePanel);
    }

    public void StartGame() {
        OpenSidePanel();
        gaugesPanel.Show(true);
        gaugesPanel.SetSpeed(gameFlow.GameBoard.PlayerPlane.Speed);
    }

    public void OnBeginMoveSelection() {
        TogglePanels(moveSelectionGO);
    }

    public bool OnMoveSelected(ManeuverData playerMove, string opponentMoveCode) {
        if (gameFlow.SetPlayerMove(playerMove, opponentMoveCode)) {
            gameFlow.PlayManeuvers();

            OpenSidePanel();

            gaugesPanel.SetSpeed(gameFlow.GameBoard.PlayerPlane.Speed);

            return true;
        }

        return false;
    }

    void OnGameStateChanged(GameFlow.GameState state) {
        if (state == GameFlow.GameState.Idle) {
            if (gameFlow.SinglePlayer) {
                moveSelectionPanel.ConfigureSinglePlayer();
            } else {
                moveSelectionPanel.ConfigureMultiPlayer();
            }
        }
    }

    void OnPlaneSpawned(Plane plane) {
        moveSelectionPanel.SetPlane(plane);
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
