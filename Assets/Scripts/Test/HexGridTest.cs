using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridTest : MonoBehaviour {
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    Vector2Int size;

    new Transform transform;
    HexGrid<float> grid;

    void Start() {
        transform = GetComponent<Transform>();
        grid = new HexGrid<float>(5, 5);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector2 pos = grid.GetCenter(new Vector2Int(x, y));

                GameObject go = Instantiate(prefab);
                go.name = string.Format("{0}, {1}", x, y);

                Transform t = go.GetComponent<Transform>();
                t.position = new Vector3(pos.x, 0, pos.y);
                t.SetParent(transform, true);
            }
        }
    }
}
