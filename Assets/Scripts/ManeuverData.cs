using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[CreateAssetMenu(menuName = "Data/Maneuver Data")]
public class ManeuverData : ScriptableObject {
    public Sprite icon;
    public bool invertIcon;
    public Vector2Int visualOffset;
    public Vector2Int finalOffset;
    public HexDirection finalDirection;
    public Spline spline;

    public HexCoord VisualOffsetHex {
        get {
            return HexCoord.FromOffset(visualOffset);
        }
    }
}
