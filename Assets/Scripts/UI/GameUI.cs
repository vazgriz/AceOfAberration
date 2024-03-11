using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    GameObject mainMenuPanel;
    [SerializeField]
    GameObject minimapPanel;
    [SerializeField]
    GameObject moveSelectionPanel;
    [SerializeField]
    GameObject moveConfirmPanel;

    void Start() {
        OpenMainMenu();
    }

    public void OpenMainMenu() {
        mainMenuPanel.SetActive(true);
    }

    public void OnBeginMoveSelection() {
        moveSelectionPanel.SetActive(true);
    }

    public void OnMoveSelected(ManeuverData data) {
        gameFlow.SetPlayerMove(data);

        moveSelectionPanel.SetActive(false);
        moveConfirmPanel.SetActive(true);
    }

    public void OnMoveConfirmed() {
        moveConfirmPanel.SetActive(false);
    }
}
