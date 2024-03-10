using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Plane : MonoBehaviour {
    HexCoord positionHex;
    HexDirection planeDirection;
    new Transform transform;

    bool maneuvering;
    ManeuverData currentManeuver;
    Spline currentSpline;

    float manueverTimer;
    Vector3 startPosition;
    Vector3 targetPosition;
    Quaternion startRotation;
    Quaternion targetRotation;

    public HexCoord PositionHex {
        get {
            return positionHex;
        }
    }
    
    public float GridSize { get; set; }
    public float ManeuverTime { get; set; }

    void Start() {
        transform = GetComponent<Transform>();
    }

    void Update() {
        UpdateManeuver();
    }

    public void PlayManeuver(ManeuverData data) {
        if (maneuvering) return;
        maneuvering = true;
        manueverTimer = 0;
        currentManeuver = data;
        currentSpline = data.spline.GetComponent<SplineContainer>().Spline;

        HexCoord localOffset = HexCoord.FromOffset(data.finalOffset);
        HexCoord offset = HexGrid.Rotate(localOffset, HexGrid.InvertDirection(planeDirection));
        positionHex += offset;

        HexDirection localDirection = data.finalDirection;
        HexDirection direction = HexGrid.RotateDirection(planeDirection, localDirection);

        Vector2 pos = HexGrid.GetCenter(positionHex) * GridSize;
        startPosition = transform.position;
        targetPosition = new Vector3(pos.x, 0, pos.y);

        float startAngle = HexGrid.GetAngle(planeDirection);
        float targetAngle = HexGrid.GetAngle(direction);
        startRotation = Quaternion.Euler(0, startAngle, 0);
        targetRotation = Quaternion.Euler(0, targetAngle, 0);

        planeDirection = direction;
    }

    void UpdateManeuver() {
        if (!maneuvering) return;

        float t = Mathf.InverseLerp(0, ManeuverTime, manueverTimer);
        float smoothT = Mathf.SmoothStep(0, 1, t);
        currentSpline.Evaluate(smoothT, out var pos, out var forward, out var up);
        transform.position = startPosition + GridSize * (startRotation * pos);
        transform.rotation = startRotation * Quaternion.LookRotation(forward, up);

        if (t >= 1) {
            maneuvering = false;
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }

        manueverTimer += Time.deltaTime;
    }
}
