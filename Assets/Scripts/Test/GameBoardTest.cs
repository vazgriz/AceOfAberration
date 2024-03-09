using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTest : MonoBehaviour {
    [SerializeField]
    float gridSize;
    [SerializeField]
    Plane plane;
    [SerializeField]
    GameObject marker;
    [SerializeField]
    Vector3 markerOffset;

    List<Transform> markers;

    void Start() {
        markers = new List<Transform>();

        const int range3Area = 37;
        for (int i = 0; i < range3Area; i++) {
            GameObject go = Instantiate(marker);
            Transform t = go.GetComponent<Transform>();
            markers.Add(t);
        }
    }

    void MoveMarkers() {

    }
}
