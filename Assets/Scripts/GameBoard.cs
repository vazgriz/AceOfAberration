using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class GameBoard {
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
        result.direction = state.direction;
        result.posIndex = GetPosToIndexMap()[state.position];

        return result;
    }

    public static PlaneState DecodeManeuver(EncodedManeuver encodedManeuver) {
        PlaneState result = new PlaneState();
        result.direction = encodedManeuver.direction;
        result.position = GetIndexToPosList()[encodedManeuver.posIndex];

        return result;
    }
}
