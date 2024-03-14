using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSelectionScreen : MonoBehaviour {
    [SerializeField]
    PlaneSelector playerSelector;
    [SerializeField]
    PlaneSelector opponentSelector;
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    GameUI gameUI;

    public void Confirm() {
        gameFlow.SetPlanes(playerSelector.GetSelection().prefab, opponentSelector.GetSelection().prefab);
        gameFlow.StartGame();
        gameUI.StartGame();
    }
}
