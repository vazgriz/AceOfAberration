using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct HexCoord {
    public float q;
    public float r;
    public float s;
}

public enum HexDirection {
    North,
    NorthWest,
    SouthWest,
    South,
    SouthEast,
    NorthEast
}

public class HexGrid<T> {
    // flat top hex
    public const float widthMult = 1;
    public const float heightMult = 1.73205080757f / 2;

    public int Width { get; private set; }
    public int Height { get; private set; }

    T[,] tiles;

    public ref T this[int x, int y] {
        get {
            return ref tiles[Mod(x, Width), y];
        }
    }

    public HexGrid(int width, int height) {
        Width = width;
        Height = height;

        tiles = new T[width, height];
    }

    static int Mod(int x, int m) {
        return (x % m + m) % m;
    }

    static readonly Vector2Int[] offsets = {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
    };

    public static Vector2Int GetOffset(HexDirection direction) {
        return offsets[(int)direction];
    }

    public Vector2Int GetNeighbor(Vector2Int pos, HexDirection direction) {
        var offset = offsets[(int)direction];
        return pos + offset;
    }

    public Vector2 GetCenter(Vector2Int pos) {
        bool even = pos.x % 2 == 0;
        float heightOffset = heightMult * (even ? 0 : 0.5f);

        float x = pos.x * 0.75f * widthMult;
        float y = pos.y * heightMult + heightOffset;

        return new Vector2(x, y);
    }

    public Vector2 GetGridCenter() {
        return new Vector3((Width * widthMult * 0.75f) / 2, (Height * heightMult) / 2);
    }
}
