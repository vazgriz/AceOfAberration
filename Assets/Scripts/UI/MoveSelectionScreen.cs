using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionScreen : MonoBehaviour {
    [SerializeField]
    GameUI gameUI;
    [SerializeField]
    Transform gridTransform;
    [SerializeField]
    GameObject playerIcon;
    [SerializeField]
    GameObject playerIconFuture;
    [SerializeField]
    GameObject opponentIcon;
    [SerializeField]
    GameObject hexGridPrefab;
    [SerializeField]
    GameObject maneuverMarkerPrefab;
    [SerializeField]
    int radius;
    [SerializeField]
    float gridSize;
    [SerializeField]
    ManeuverIcon infoIcon;
    [SerializeField]
    TextMeshProUGUI infoTitle;
    [SerializeField]
    TextMeshProUGUI infoDescription;
    [SerializeField]
    GameObject confirmPanel;

    new Transform transform;
    List<GameObject> gridIcons;
    List<ManeuverIcon> maneuverIcons;

    Transform playerIconFutureTransform;
    Transform opponentIconTransform;

    ManeuverData lockedManeuver;

    Plane plane;

    void Start() {
        transform = GetComponent<Transform>();
        gridIcons = new List<GameObject>();
        maneuverIcons = new List<ManeuverIcon>();

        playerIconFutureTransform = playerIconFuture.GetComponent<Transform>();
        opponentIconTransform = opponentIcon.GetComponent<Transform>();

        playerIconFuture.SetActive(false);
        opponentIcon.SetActive(false);

        infoIcon.ShowImage(false);
        infoTitle.text = "";
        infoDescription.text = "";

        for (int i = 0; i <= radius; i++) {
            foreach (var pos in HexGrid.EnumerateRing(new HexCoord(), i)) {
                var go = Instantiate(hexGridPrefab);
                var t = go.GetComponent<RectTransform>();

                t.SetParent(gridTransform);
                t.localPosition = HexGrid.GetCenter(pos) * gridSize;

                gridIcons.Add(go);
            }
        }
    }

    public void SetPlane(Plane plane) {
        if (plane == null) return;
        if (plane.ManeuverList == null) return;

        this.plane = plane;
        ManeuverList list = plane.ManeuverList;

        foreach (var maneuver in list.maneuvers) {
            var go = Instantiate(maneuverMarkerPrefab);
            var t = go.GetComponent<Transform>();
            var icon = go.GetComponent<ManeuverIcon>();

            icon.SetManeuverData(maneuver);
            icon.OnClicked += OnClick;
            icon.OnHoverEnter += OnHoverEnter;
            icon.OnHoverExit += OnHoverExit;

            HexDirection direction = (HexDirection)HexGrid.Mod((int)maneuver.finalDirection + 3, 6);
            Vector2 position = HexGrid.GetCenter(maneuver.visualOffset) + HexGrid.GetEdgeCenter(direction);

            t.SetParent(transform);
            t.localPosition = gridSize * position;

            maneuverIcons.Add(icon);
        }
    }

    public void ResetScreen() {
        confirmPanel.SetActive(false);
        playerIconFuture.SetActive(false);
        opponentIcon.SetActive(false);
    }

    public void ShowScreen() {

    }

    public void ConfirmManeuver() {
        if (lockedManeuver == null) return;

        gameUI.OnMoveSelected(lockedManeuver);
        lockedManeuver = null;
    }

    void OnClick(ManeuverData maneuver) {
        lockedManeuver = maneuver;

        confirmPanel.SetActive(true);
    }

    void OnHoverEnter(ManeuverData maneuver) {
        infoIcon.ShowImage(true);
        infoIcon.SetManeuverData(maneuver);
        infoTitle.text = maneuver.infoTitle;
        infoDescription.text = maneuver.infoDescription;

        HexDirection direction = maneuver.finalDirection;
        Vector2 position = HexGrid.GetCenter(maneuver.finalOffset);
        float angle = HexGrid.GetAngle(HexGrid.InvertDirection(direction));

        playerIconFutureTransform.localPosition = gridSize * position;
        playerIconFutureTransform.rotation = Quaternion.Euler(0, 0, angle);
        playerIconFuture.SetActive(true);

        if (lockedManeuver != null && lockedManeuver != maneuver) {
            lockedManeuver = null;
            confirmPanel.SetActive(false);
        }
    }

    void OnHoverExit(ManeuverData maneuver) {
        if (lockedManeuver == null) {
            playerIconFuture.SetActive(false);
        }
    }
}
