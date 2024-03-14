using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTest : MonoBehaviour {
    [SerializeField]
    float gridSize;
    [SerializeField]
    Plane plane;
    [SerializeField]
    GameObject markerPrefab;
    [SerializeField]
    Vector3 markerOffset;
    [SerializeField]
    float maneuverTime;
    [SerializeField]
    new Camera camera;
    [SerializeField]
    Vector3 cameraOffset;

    List<Transform> markers;
    Transform cameraTransform;

    const int radius = 3;

    void Start() {
        markers = new List<Transform>();
        cameraTransform = camera.GetComponent<Transform>();

        cameraTransform.SetParent(plane.GetComponent<Transform>());
        cameraTransform.localPosition =  cameraOffset;
        cameraTransform.localRotation = Quaternion.identity;

        for (int i = 0; i <= radius; i++) {
            foreach (var offset in HexGrid.EnumerateRing(new HexCoord(), i)) {
                var go = Instantiate(markerPrefab);
                var t = go.GetComponent<Transform>();
                markers.Add(t);
            }
        }

        plane.GridSize = gridSize;
        plane.ManeuverTime = maneuverTime;

        MoveMarkers();
    }

    void MoveMarkers() {
        int index = 0;

        for (int i = 0; i <= radius; i++) {
            foreach (var offset in HexGrid.EnumerateRing(plane.PositionHex, i)) {
                var marker = markers[index];
                var pos = HexGrid.GetCenter(HexCoord.ToOffset(offset));
                marker.position = markerOffset + new Vector3(pos.x, 0, pos.y) * gridSize;
                index++;
            }
        }
    }

    public void PlayManeuver(ManeuverData data) {
        ManeuverState state = GameBoard.CalculateManeuver(data, plane.Direction, plane.Speed);
        plane.PlayManeuver(state.finalState, data);
        MoveMarkers();
    }
}
