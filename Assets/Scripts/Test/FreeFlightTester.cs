using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlightTester : MonoBehaviour {
    [SerializeField]
    GameUI gameUI;
    [SerializeField]
    GameObject planePrefab;

    GameFlow gameFlow;

    void Start() {
        StartCoroutine(StartTest());
    }

    IEnumerator StartTest() {
        yield return null;
        gameFlow = GetComponent<GameFlow>();
        gameFlow.SinglePlayer = true;
        gameFlow.SetPlanes(planePrefab, null);
        gameFlow.StartGame();
    }
}
