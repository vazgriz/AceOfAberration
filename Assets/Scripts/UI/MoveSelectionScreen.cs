using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSelectionScreen : MonoBehaviour {
    [SerializeField]
    GameUI gameUI;
    [SerializeField]
    AudioSource errorSound;
    [SerializeField]
    Transform gridTransform;
    [SerializeField]
    GameObject playerIcon;
    [SerializeField]
    GameObject playerIconFuture;
    [SerializeField]
    GameObject opponentIcon;
    [SerializeField]
    GameObject opponentIconFuture;
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
    SidePanel sidePanel;
    [SerializeField]
    GameObject confirmPanelSP;
    [SerializeField]
    GameObject confirmPanelMP;
    [SerializeField]
    TextMeshProUGUI playerMoveLabel;
    [SerializeField]
    TMP_InputField opponentMoveInput;

    GameObject selfGO;
    GameObject confirmPanel;
    bool singlePlayer;

    bool init;
    new Transform transform;
    List<GameObject> gridIcons;
    List<ManeuverIcon> maneuverIcons;

    Transform playerIconFutureTransform;
    Transform opponentIconTransform;

    ManeuverData selectedManeuver;

    Plane playerPlane;
    Plane opponentPlane;

    void Init() {
        if (init) return;
        init = true;

        selfGO = gameObject;
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

        confirmPanel = confirmPanelSP;
    }
        
    void Start() {
        Init();

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

        Init();

        playerPlane = plane;
        ManeuverList list = plane.ManeuverList;

        foreach (var maneuver in list.maneuvers) {
            var go = Instantiate(maneuverMarkerPrefab);
            var t = go.GetComponent<Transform>();
            var icon = go.GetComponent<ManeuverIcon>();

            icon.SetManeuverData(maneuver);
            icon.OnClicked += OnClick;
            icon.OnHoverEnter += OnHoverEnter;
            icon.OnHoverExit += OnHoverExit;

            HexDirection direction = HexGrid.RotateDirection(maneuver.finalDirection, HexDirection.South);
            Vector2 position = HexGrid.GetCenter(maneuver.visualOffset) + HexGrid.GetEdgeCenter(direction);

            t.SetParent(transform);
            t.localPosition = gridSize * position;

            maneuverIcons.Add(icon);
        }
    }

    public void SetOpponentPlane(Plane plane) {
        opponentPlane = plane;
    }

    public void ResetScreen() {
        confirmPanelSP.SetActive(false);
        confirmPanelMP.SetActive(false);
        playerIconFuture.SetActive(false);
        opponentIcon.SetActive(false);
        opponentMoveInput.text = "";

        DisplayMoves();
        DisplayOpponentPlane();
    }

    void DisplayMoves() {
        foreach (var icon in maneuverIcons) {
            ManeuverData data = icon.ManeuverData;
            bool isValid = playerPlane.IsManeuverValid(data);
            icon.gameObject.SetActive(isValid);
        }
    }

    void DisplayOpponentPlane() {
        if (opponentPlane == null) return;

        HexCoord relativePos = opponentPlane.PositionHex - playerPlane.PositionHex;
        HexDirection direction = opponentPlane.Direction;
        Vector2 position = HexGrid.GetCenter(relativePos);
        float angle = HexGrid.GetAngle(HexGrid.InvertDirection(direction));

        playerIconFutureTransform.localPosition = gridSize * position;
        playerIconFutureTransform.rotation = Quaternion.Euler(0, 0, angle);
        playerIconFuture.SetActive(true);
    }

    public void ConfigureSinglePlayer() {
        confirmPanel = confirmPanelSP;
        singlePlayer = true;
    }

    public void ConfigureMultiPlayer() {
        confirmPanel = confirmPanelMP;
        singlePlayer = false;
    }

    public bool ShowScreen(bool value) {
        if (value && playerPlane.Maneuvering) return false;

        if (value) {
            ResetScreen();
        }

        selfGO.SetActive(value);

        return true;
    }

    public void ConfirmManeuver() {
        if (singlePlayer) {
            ConfirmSinglePlayer();
        } else {
            ConfirmMultiPlayer();
        }
    }

    void ConfirmSinglePlayer() {
        if (selectedManeuver == null) return;

        gameUI.OnMoveSelected(selectedManeuver, null);
        selectedManeuver = null;
    }

    void ConfirmMultiPlayer() {
        if (selectedManeuver == null) return;

        if (gameUI.OnMoveSelected(selectedManeuver, opponentMoveInput.text)) {
            selectedManeuver = null;
        } else {
            errorSound.Play();
        }
    }

    public void ExitScreen() {
        ShowScreen(false);
        sidePanel.ShowScreen(true);
    }

    void DisplayInfo(ManeuverData maneuver) {
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
    }

    void OnClick(ManeuverData maneuver) {
        selectedManeuver = maneuver;

        confirmPanel.SetActive(true);

        if (!singlePlayer) {
            ManeuverState state = GameBoard.CalculateManeuver(selectedManeuver, playerPlane.Direction, playerPlane.Speed);

            EncodedManeuver encoded = GameBoard.EncodeManeuver(state.finalState);
            playerMoveLabel.text = GameBoard.PrintManeuver(encoded);
        }
    }

    void OnHoverEnter(ManeuverData maneuver) {
        DisplayInfo(maneuver);
    }

    void OnHoverExit(ManeuverData maneuver) {
        if (selectedManeuver == null) {
            playerIconFuture.SetActive(false);
        } else {
            DisplayInfo(selectedManeuver);
        }
    }
}
