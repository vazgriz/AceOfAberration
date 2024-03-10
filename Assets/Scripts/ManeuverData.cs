using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Maneuver Data")]
public class ManeuverData : ScriptableObject {
    public Sprite icon;
    public Vector2Int visualOffset;
    public Vector2Int finalOffset;
    public HexDirection finalDirection;

    public HexCoord VisualOffsetHex {
        get {
            return HexCoord.FromOffset(visualOffset);
        }
    }
}
