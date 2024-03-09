using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class HexGridTest {
    [Test]
    public void ConversionTest() {
        Assert.AreEqual(new HexCoord(-2, -1,  3), HexCoord.FromOffset(new Vector2Int(-2, -2)));
        Assert.AreEqual(new HexCoord( 1, -2,  1), HexCoord.FromOffset(new Vector2Int( 1, -2)));
        Assert.AreEqual(new HexCoord( 1,  2, -3), HexCoord.FromOffset(new Vector2Int( 1,  2)));
        Assert.AreEqual(new HexCoord(-2,  3, -1), HexCoord.FromOffset(new Vector2Int(-2,  2)));
    }

    [Test]
    public void RotationTest() {
        HexCoord pos = new HexCoord(4, 3, 2);

        Assert.AreEqual(new HexCoord( 4,  3,  2), HexGrid.Rotate(pos, HexDirection.North));
        Assert.AreEqual(new HexCoord(-2, -4, -3), HexGrid.Rotate(pos, HexDirection.NorthWest));
        Assert.AreEqual(new HexCoord( 3,  2,  4), HexGrid.Rotate(pos, HexDirection.SouthWest));
        Assert.AreEqual(new HexCoord(-4, -3, -2), HexGrid.Rotate(pos, HexDirection.South));
        Assert.AreEqual(new HexCoord( 2,  4,  3), HexGrid.Rotate(pos, HexDirection.SouthEast));
        Assert.AreEqual(new HexCoord(-3, -2, -4), HexGrid.Rotate(pos, HexDirection.NorthEast));
    }
}
