using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Maneuver Data")]
public class ManeuverData : ScriptableObject {
    public Sprite icon;
    public bool invertIcon;
    public Vector2Int visualOffset;
    public Vector2Int finalOffset;
    public HexDirection finalDirection;
    public Plane.PlaneSpeed finalSpeed;
    public GameObject spline;
    public bool speedOverride;
    public AnimationCurve speedCurve;
    public Plane.PlaneSpeed minSpeed;
    public Plane.PlaneSpeed maxSpeed;
    public bool isSpecial;

    [TextArea]
    public string infoTitle;
    [TextArea]
    public string infoDescription;

    public HexCoord VisualOffsetHex {
        get {
            return HexCoord.FromOffset(visualOffset);
        }
    }
}
