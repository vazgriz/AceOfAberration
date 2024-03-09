using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCoord {
    public int q;
    public int r;
    public int s;

    public HexCoord(int q, int r, int s) {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public static bool operator == (HexCoord a, HexCoord b) {
        return a.q == b.q & a.r == b.r & a.s == b.s;
    }

    public static bool operator != (HexCoord a, HexCoord b) {
        return a.q != b.q | a.r != b.r | a.s != b.s;
    }

    public override readonly int GetHashCode() {
        return q.GetHashCode() ^ (r.GetHashCode() << 8) ^ (s.GetHashCode() << 16);
    }

    public override readonly bool Equals(object obj) {
        if (obj is HexCoord other) {
            return this == other;
        }

        return false;
    }

    public override string ToString() {
        return string.Format("[{0}, {1} {2}]", q, r, s);
    }

    public static HexCoord operator + (HexCoord a, HexCoord b) {
        return new HexCoord(a.q + b.q, a.r + b.r, a.s + b.s);
    }

    public static HexCoord operator  * (HexCoord a, int b) {
        return new HexCoord(a.q * b, a.r * b, a.s * b);
    }

    public static HexCoord FromOffset(Vector2Int pos) {
        int q = pos.x;
        int r = pos.y - (pos.x - (pos.x & 1)) / 2;
        int s = 0 - q - r;

        HexCoord result = new HexCoord(q, r, s);
        return result;
    }

    public static Vector2Int ToOffset(HexCoord hex) {
        int col = hex.q;
        int row = hex.r + (hex.q - (hex.q & 1)) / 2;

        return new Vector2Int(col, row);
    }
}

public enum HexDirection {
    North,
    NorthWest,
    SouthWest,
    South,
    SouthEast,
    NorthEast
}

public abstract class HexGrid {
    // flat top hex
    public const float widthMult = 1;
    public const float heightMult = 1.73205080757f / 2;

    public static int Mod(int x, int m) {
        return (x % m + m) % m;
    }

    static readonly Vector2Int[] neighborsOffset = {
        new Vector2Int( 0, -1),
        new Vector2Int(-1,  0),
        new Vector2Int(-1,  1),
        new Vector2Int( 0,  1),
        new Vector2Int( 1,  0),
        new Vector2Int( 1, -1),
    };

    static readonly HexCoord[] neighborsCubic = {
        new HexCoord( 0, -1,  1),
        new HexCoord(-1,  0,  1),
        new HexCoord(-1,  1,  0),
        new HexCoord( 0,  1, -1),
        new HexCoord( 1,  0, -1),
        new HexCoord( 1, -1,  0)
    };

    public static Vector2Int GetOffset(HexDirection direction) {
        return neighborsOffset[(int)direction];
    }

    public static Vector2Int GetNeighborOffset(Vector2Int pos, HexDirection direction) {
        var offset = neighborsOffset[(int)direction];
        return pos + offset;
    }

    public static HexCoord GetOffsetCubic(HexDirection direction) {
        return neighborsCubic[(int)direction];
    }

    public static HexCoord GetNeighborCubic(HexCoord pos, HexDirection direction) {
        var offset = neighborsCubic[(int)direction];
        return pos + offset;
    }

    public static Vector2 GetCenter(Vector2Int pos) {
        bool even = pos.x % 2 == 0;
        float heightOffset = heightMult * (even ? 0 : 0.5f);

        float x = pos.x * 0.75f * widthMult;
        float y = pos.y * heightMult + heightOffset;

        return new Vector2(x, y);
    }

    public static RingEnumerator EnumerateRing(HexCoord center, int radius) {
        return new RingEnumerator(center, radius);
    }

    /// <summary>
    /// Rotate <i>position</i> around the origin based on the angle between North and <i>direction</i>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static HexCoord Rotate(HexCoord position, HexDirection direction) {
        int rotations = (int)direction;

        switch (rotations) {
            case 0:
                return position;
            case 1:
                return new HexCoord(-position.s, -position.q, -position.r);
            case 2:
                return new HexCoord(position.r, position.s, position.q);
            case 3:
                return new HexCoord(-position.q, -position.r, -position.s);
            case 4:
                return new HexCoord(position.s, position.q, position.r);
            case 5:
                return new HexCoord(-position.r, -position.s, -position.q);
            default:
                throw new ArgumentException(nameof(direction));
        }
    }

    public struct RingEnumerator : IEnumerator<HexCoord>, IEnumerable<HexCoord> {
        int radius;
        int count;
        int total;
        HexDirection direction;
        HexCoord start;
        HexCoord current;
        HexCoord last;

        public HexCoord Current => last;

        object IEnumerator.Current => throw new NotImplementedException();

        public RingEnumerator(HexCoord center, int radius) {
            if (radius < 0) throw new InvalidOperationException(nameof(radius));

            this.radius = radius;
            count = 0;
            total = 0;
            direction = HexDirection.SouthWest;

            HexCoord offset = HexGrid.GetOffsetCubic(HexDirection.North) * radius;
            start = center + offset;
            current = start;
            last = start;
        }

        public bool MoveNext() {
            if (total > 0 && total >= radius * 6) return false;

            if (count == radius) {
                // start a new direction
                direction = (HexDirection)HexGrid.Mod(((int)direction + 1), 6);
                count = 0;
            }

            last = current;
            HexCoord offset = HexGrid.GetOffsetCubic(direction);
            current += offset;

            count++;
            total++;

            return true;
        }

        public void Reset() {
            current = start;
            count = 0;
        }

        public void Dispose() {
            // do nothing
        }

        public IEnumerator<HexCoord> GetEnumerator() {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}

public class HexGridData<T> : HexGrid {
    public int Width { get; private set; }
    public int Height { get; private set; }

    T[,] tiles;

    public ref T this[int x, int y] {
        get {
            return ref tiles[Mod(x, Width), y];
        }
    }

    public HexGridData(int width, int height) {
        Width = width;
        Height = height;

        tiles = new T[width, height];
    }

    public Vector2 GetGridCenter() {
        return new Vector3((Width * widthMult * 0.75f) / 2, (Height * heightMult) / 2);
    }
}
