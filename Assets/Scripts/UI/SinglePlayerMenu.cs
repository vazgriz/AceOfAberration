using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerMenu : MonoBehaviour {
    [SerializeField]
    GameFlow gameFlow;
    [SerializeField]
    GameUI gameUI;
    [SerializeField]
    GameObject planePrefab1;
    [SerializeField]
    GameObject planePrefab2;

    public void OpenPlayVsAIMenu() {

    }

    public void OpenFreeFlight1() {
        StartFreeFlight(planePrefab1);
    }

    public void OpenFreeFlight2() {
        StartFreeFlight(planePrefab2);
    }

    void StartFreeFlight(GameObject prefab) {
        gameFlow.SinglePlayer = true;
        gameFlow.SetPlanes(prefab, null);
        gameFlow.StartGame();
        gameObject.SetActive(false);
        gameUI.StartGame();
    }
}
