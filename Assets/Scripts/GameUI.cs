using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    GameObject moveSelectionWindow;
    [SerializeField]
    GameObject moveConfirmWindow;

    public void OnBeginMoveSelection() {
        moveSelectionWindow.SetActive(true);
    }

    public void OnMoveSelected(ManeuverData data) {
        gameFlow.SelectPlayerMove(data);

        moveSelectionWindow.SetActive(false);
        moveConfirmWindow.SetActive(true);
    }

    public void OnMoveConfirmed() {
        moveConfirmWindow.SetActive(false);
    }
}
