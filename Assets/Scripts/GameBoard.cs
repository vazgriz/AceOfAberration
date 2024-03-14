using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameFlow;

public struct PlaneState {
    public HexCoord position;
    public HexDirection direction;
    public Plane.PlaneSpeed speed;
}

public struct ManeuverState {
    public ManeuverData maneuver;
    public PlaneState initialState;
    public PlaneState finalState;
}

public struct EncodedManeuver {
    public HexDirection direction;
    public int posIndex;
    public int speed;
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
    [SerializeField]
    Plane.PlaneSpeed startSpeed;
    [SerializeField]
    ManeuverList maneuverList;

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

    public Plane PlayerPlane {
        get {
            return playerPlane;
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
        plane.PositionHex = HexCoord.FromOffset(position);
        plane.Direction = direction;
        plane.Speed = startSpeed;

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

    public bool ValidateManeuvers(ManeuverData playerMove, string opponentMoveCode, out PlaneState opponentState, out ManeuverData opponentMove) {
        opponentMove = null;
        opponentState = new PlaneState();

        if (playerMove == null) {
            return false;
        }

        if (opponentMoveCode != null) {
            if (opponentMoveCode.Length == 0) {
                return false;
            }

            EncodedManeuver? encoded = ParseManeuver(opponentMoveCode);
            if (!encoded.HasValue) return false;

            PlaneState oppositeWorldSpace = DecodeManeuver(encoded.Value);
            PlaneState worldState = RotateState(oppositeWorldSpace, HexDirection.South);   // rotate opponent world space by 180 degrees
            PlaneState localState = WorldToLocal(worldState, opponentPlane.Direction);

            opponentState = worldState;
            opponentMove = GetManeuverFromState(localState, maneuverList);
        }

        return true;
    }

    public void PlayManeuvers(ManeuverData playerMove, PlaneState opponentState, ManeuverData opponentMove) {
        if (playerPlane != null && playerMove != null) {
            PlaneState state = CalculateManeuver(playerMove, playerPlane.Direction, playerPlane.Speed).finalState;
            playerPlane.PlayManeuver(state, playerMove);
        }

        if (opponentPlane != null) {
            opponentPlane.PlayManeuver(opponentState, opponentMove);
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

    public static ManeuverData GetManeuverFromState(PlaneState state, ManeuverList list) {
        foreach (var data in list.maneuvers) {
            HexCoord coord = HexCoord.FromOffset(data.finalOffset);
            if (state.position == coord
                && state.direction == data.finalDirection
                && state.speed == data.finalSpeed)
            {
                return data;
            }
        }

        return null;
    }

    /// <summary>
    /// Calculates the result of a maneuver relative (plane's position is considered to be the origin)
    /// </summary>
    /// <param name="maneuver"></param>
    /// <param name="direction"></param>
    /// <returns>The position offset and world direction resulting from the maneuver</returns>
    public static ManeuverState CalculateManeuver(ManeuverData maneuver, HexDirection direction, Plane.PlaneSpeed speed) {
        ManeuverState state = new ManeuverState();
        state.maneuver = maneuver;
        state.initialState.position = new HexCoord();
        state.initialState.direction = direction;
        state.initialState.speed = speed;

        HexCoord localOffset = HexCoord.FromOffset(maneuver.finalOffset);
        HexCoord offset = HexGrid.Rotate(localOffset, HexGrid.InvertDirection(direction));
        state.finalState.position = offset;

        HexDirection localDirection = maneuver.finalDirection;
        state.finalState.direction = HexGrid.RotateDirection(direction, localDirection);

        state.finalState.speed = maneuver.finalSpeed;

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
        result.speed = (int)state.speed;

        return result;
    }

    public static PlaneState DecodeManeuver(EncodedManeuver encodedManeuver) {
        PlaneState result = new PlaneState();
        HexDirection adjustedDirection = HexGrid.RotateDirection(encodedManeuver.direction, HexDirection.South);
        HexCoord adjustedCoord = HexGrid.Rotate(GetIndexToPosList()[encodedManeuver.posIndex], HexDirection.South);

        result.direction = adjustedDirection;
        result.position = adjustedCoord;
        result.speed = (Plane.PlaneSpeed)encodedManeuver.speed;

        return result;
    }

    public static int Linearize(EncodedManeuver encoded) {
        return encoded.posIndex * (6 * 3)
            + (int)encoded.direction * (3)
            + encoded.speed;
    }

    public static void Delinearize(out EncodedManeuver result, int linear) {
        EncodedManeuver encoded = new EncodedManeuver();
        encoded.speed = linear % 3;
        linear /= 3;
        encoded.direction = (HexDirection)(linear % 6);
        linear /= 6;
        encoded.posIndex = linear;

        result = encoded;
    }

    public static string PrintManeuver(EncodedManeuver encoded) {
        int linear = Linearize(encoded);
        int b = linear % 26;
        int a = linear / 26;
        return string.Format("{0}{1}", (char)('A' + a), (char)('A' + b));
    }

    public static EncodedManeuver? ParseManeuver(string code) {
        if (code.Length != 2) {
            return null;
        }

        char c0 = char.ToUpper(code[0]);
        char c1 = char.ToUpper(code[1]);

        if (c0 >= 'A' && c0 <= 'Z' && c1 >= 'A' && c1 <= 'Z') {
            int a = c0 - 'A';
            int b = c1 - 'A';
            int linear = (a * 26) + b;
            EncodedManeuver result;
            Delinearize(out result, linear);
            return result;
        }

        return null;
    }

    public static PlaneState RotateState(PlaneState state, HexDirection direction) {
        PlaneState result = state;
        result.position = HexGrid.Rotate(state.position, direction);
        result.direction = HexGrid.RotateDirection(state.direction, direction);

        return result;
    }

    public static PlaneState LocalToWorld(PlaneState state, HexDirection direction) {
        return RotateState(state, direction);
    }

    public static PlaneState WorldToLocal(PlaneState state, HexDirection direction) {
        PlaneState result = state;
        HexDirection invRotation = HexGrid.InvertDirection(direction);
        result.position = HexGrid.Rotate(state.position, direction);
        result.direction = HexGrid.RotateDirection(state.direction, invRotation);

        return result;
    }
}
