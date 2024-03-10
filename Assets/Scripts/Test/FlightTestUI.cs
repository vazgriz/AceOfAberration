using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTestUI : MonoBehaviour {
    [SerializeField]
    GameObject iconPrefab;
    [SerializeField]
    List<ManeuverData> maneuvers;
    [SerializeField]
    float gridSize;
    [SerializeField]
    GameBoardTest gameBoard;

    [SerializeField]
    RectTransform maneuverHolder;

    void Start() {
        foreach (var maneuver in maneuvers) {
            var offset = maneuver.visualOffset;
            var go = CreateManeuverIcon(maneuver);
            var t = go.GetComponent<RectTransform>();

            t.SetParent(maneuverHolder);
            t.localPosition = HexGrid.GetCenter(offset) * gridSize;
        }
    }

    GameObject CreateManeuverIcon(ManeuverData data) {
        var go = Instantiate(iconPrefab);
        var maneuverIcon = go.GetComponent<ManeuverIcon>();
        maneuverIcon.SetManeuverData(data);
        maneuverIcon.OnClicked += OnClick;

        return go;
    }

    void OnClick(ManeuverData data) {
        gameBoard.PlayManeuver(data);
    }
}
