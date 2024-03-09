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

    List<Transform> markers;
    HexCoord planePos;

    const int radius = 3;

    void Start() {
        markers = new List<Transform>();

        for (int i = 0; i <= 3; i++) {
            foreach (var offset in HexGrid.EnumerateRing(new HexCoord(), i)) {
                var go = Instantiate(markerPrefab);
                var t = go.GetComponent<Transform>();
                markers.Add(t);
            }
        }

        MoveMarkers();
    }

    void MoveMarkers() {
        int index = 0;

        for (int i = 0; i <= 3; i++) {
            foreach (var offset in HexGrid.EnumerateRing(planePos, i)) {
                var marker = markers[index];
                var pos = HexGrid.GetCenter(HexCoord.ToOffset(offset));
                marker.position = new Vector3(pos.x, 0, pos.y) * gridSize;
                index++;
            }
        }
    }
}
