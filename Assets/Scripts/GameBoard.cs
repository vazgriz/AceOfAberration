using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameFlow;

public struct PlaneState {
    public HexCoord position;
    public HexDirection direction;
}

public struct ManeuverState {
    public ManeuverData maneuver;
    public PlaneState initialState;
    public PlaneState finalState;
}

public struct EncodedManeuver {
    public HexDirection direction;
    public int posIndex;
}

public class GameBoard : MonoBehaviour {
    [SerializeField]
    float gridSize;
    [SerializeField]
    float maneuverTime;
    [SerializeField]
    new CameraController camera;
    [SerializeField]
    Vector2Int playerStartOffset;
    [SerializeField]
    Vector2Int opponentStartOffset;

    Plane playerPlane;
    Plane opponentPlane;

    bool playingManeuver;
    float maneuverTimer;

    public float ManeuverTime {
        get {
            return maneuverTime;
        }
    }

    public float ManeuverTimer {
        get {
            return maneuverTimer;
        }
    }

    public float GridSize {
        get {
            return gridSize;
        }
    }

    public bool PlayingManeuver {
        get {
            return playingManeuver;
        }
    }

    void Update() {
        UpdateManeuver();
    }

    Plane SpawnPlane(GameObject prefab, Vector2Int position, HexDirection direction) {
        if (prefab == null) return null;

        var go = Instantiate(prefab);
        var plane = go.GetComponent<Plane>();

        plane.Init();

        plane.ManeuverTime = ManeuverTime;
        plane.GridSize = GridSize;
        plane.PositionHex = HexCoord.FromOffset(playerStartOffset);

        return plane;
    }

    public void SetPlanes(GameObject playerPrefab, GameObject opponentPrefab) {
        playerPlane = SpawnPlane(playerPrefab, playerStartOffset, HexDirection.North);
        opponentPlane = SpawnPlane(opponentPrefab, opponentStartOffset, HexDirection.South);

        camera.SetAttachment(playerPlane.transform);
    }

    public void ClearGameState() {
        ClearPlane(ref playerPlane);
        ClearPlane(ref opponentPlane);
    }

    void ClearPlane(ref Plane plane) {
        if (plane == null) return;
        Destroy(plane.gameObject);
        plane = null;
    }

    public void PlayManeuvers(ManeuverData playerMove, ManeuverData opponentMove) {
        if (playerPlane != null && playerMove != null) {
            playerPlane.PlayManeuver(playerMove);
        }

        if (opponentPlane != null && opponentMove != null) {
            opponentPlane.PlayManeuver(opponentMove);
        }

        playingManeuver = true;
        maneuverTimer = 0;
    }

    void UpdateManeuver() {
        if (!playingManeuver) return;

        if (maneuverTimer > ManeuverTime) {
            playingManeuver = false;
        }

        maneuverTimer += Time.deltaTime;
    }

    /// <summary>
    /// Calculates the result of a maneuver relative (plane's position is considered to be the origin)
    /// </summary>
    /// <param name="maneuver"></param>
    /// <param name="direction"></param>
    /// <returns>The position offset and world direction resulting from the maneuver</returns>
    public static ManeuverState CalculateManeuver(ManeuverData maneuver, HexDirection direction) {
        ManeuverState state = new ManeuverState();
        state.maneuver = maneuver;
        state.initialState.position = new HexCoord();
        state.initialState.direction = direction;

        HexCoord localOffset = HexCoord.FromOffset(maneuver.finalOffset);
        HexCoord offset = HexGrid.Rotate(localOffset, HexGrid.InvertDirection(direction));
        state.finalState.position = offset;

        HexDirection localDirection = maneuver.finalDirection;
        state.finalState.direction = HexGrid.RotateDirection(direction, localDirection);

        return state;
    }

    static Dictionary<HexCoord, int> posToIndexMap;
    static List<HexCoord> indexToPosList;

    static void InitPosMap() {
        if (posToIndexMap != null) return;

        posToIndexMap = new Dictionary<HexCoord, int>();
        indexToPosList = new List<HexCoord>();

        const int radius = 3;

        for (int i = 0; i <= radius; i++) {
            foreach (var pos in HexGrid.EnumerateRing(new HexCoord(), i)) {
                posToIndexMap.Add(pos, indexToPosList.Count);
                indexToPosList.Add(pos);
            }
        }
    }

    static Dictionary<HexCoord, int> GetPosToIndexMap() {
        InitPosMap();
        return posToIndexMap;
    }

    static List<HexCoord> GetIndexToPosList() {
        InitPosMap();
        return indexToPosList;
    }

    public static EncodedManeuver EncodeManeuver(PlaneState state) {
        EncodedManeuver result = new EncodedManeuver();
        HexDirection adjustedRotation = HexGrid.RotateDirection(state.direction, HexDirection.South);
        HexCoord adjustedCoord = HexGrid.Rotate(state.position, HexDirection.South);

        result.direction = adjustedRotation;
        result.posIndex = GetPosToIndexMap()[adjustedCoord];

        return result;
    }

    public static PlaneState DecodeManeuver(EncodedManeuver encodedManeuver) {
        PlaneState result = new PlaneState();
        HexDirection adjustedDirection = HexGrid.RotateDirection(encodedManeuver.direction, HexDirection.South);
        HexCoord adjustedCoord = HexGrid.Rotate(GetIndexToPosList()[encodedManeuver.posIndex], HexDirection.South);

        result.direction = adjustedDirection;
        result.position = adjustedCoord;

        return result;
    }
}
