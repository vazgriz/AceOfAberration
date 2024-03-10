using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {
    HexCoord positionHex;
    HexDirection planeDirection;
    Transform planeTransform;

    bool maneuvering;
    float manueverTimer;
    Vector3 startPosition;
    Vector3 targetPosition;
    float startAngle;
    float targetAngle;

    public HexCoord PositionHex {
        get {
            return positionHex;
        }
    }
    
    public float GridSize { get; set; }
    public float ManeuverTime { get; set; }

    void Start() {
        planeTransform = GetComponent<Transform>();
    }

    void Update() {
        UpdateManeuver();
    }

    public void PlayManeuver(ManeuverData data) {
        if (maneuvering) return;
        maneuvering = true;
        manueverTimer = 0;

        HexCoord localOffset = HexCoord.FromOffset(data.finalOffset);
        HexCoord offset = HexGrid.Rotate(localOffset, HexGrid.InvertDirection(planeDirection));
        positionHex += offset;

        HexDirection localDirection = data.finalDirection;
        HexDirection direction = HexGrid.RotateDirection(planeDirection, localDirection);

        Vector2 pos = HexGrid.GetCenter(positionHex) * GridSize;
        startPosition = planeTransform.position;
        targetPosition = new Vector3(pos.x, 0, pos.y);

        startAngle = HexGrid.GetAngle(planeDirection);
        targetAngle = HexGrid.GetAngle(direction);

        planeDirection = direction;
    }

    void UpdateManeuver() {
        if (!maneuvering) return;

        float t = Mathf.InverseLerp(0, ManeuverTime, manueverTimer);
        planeTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
        planeTransform.rotation = Quaternion.Slerp(Quaternion.Euler(0, startAngle, 0), Quaternion.Euler(0, targetAngle, 0), t);

        if (t >= 1) {
            maneuvering = false;
        }

        manueverTimer += Time.deltaTime;
    }
}
