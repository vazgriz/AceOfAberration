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
        HexCoord pos = new HexCoord(-5, 2, 3);

        Assert.AreEqual(new HexCoord(-5,  2,  3), HexGrid.Rotate(pos, HexDirection.North));
        Assert.AreEqual(new HexCoord(-3,  5, -2), HexGrid.Rotate(pos, HexDirection.NorthWest));
        Assert.AreEqual(new HexCoord( 2,  3, -5), HexGrid.Rotate(pos, HexDirection.SouthWest));
        Assert.AreEqual(new HexCoord( 5, -2, -3), HexGrid.Rotate(pos, HexDirection.South));
        Assert.AreEqual(new HexCoord( 3, -5,  2), HexGrid.Rotate(pos, HexDirection.SouthEast));
        Assert.AreEqual(new HexCoord(-2, -3,  5), HexGrid.Rotate(pos, HexDirection.NorthEast));
    }

    [Test]
    public void RotateDirectionTest() {
        HexDirection[] directions = new HexDirection[] {
            HexDirection.North,
            HexDirection.NorthWest,
            HexDirection.SouthWest,
            HexDirection.South,
            HexDirection.SouthEast,
            HexDirection.NorthEast,
        };

        HexDirection[] expected = new HexDirection[] {
            HexDirection.North,
            HexDirection.NorthWest,
            HexDirection.SouthWest,
            HexDirection.South,
            HexDirection.SouthEast,
            HexDirection.NorthEast,

            HexDirection.NorthWest,
            HexDirection.SouthWest,
            HexDirection.South,
            HexDirection.SouthEast,
            HexDirection.NorthEast,
            HexDirection.North,

            HexDirection.SouthWest,
            HexDirection.South,
            HexDirection.SouthEast,
            HexDirection.NorthEast,
            HexDirection.North,
            HexDirection.NorthWest,

            HexDirection.South,
            HexDirection.SouthEast,
            HexDirection.NorthEast,
            HexDirection.North,
            HexDirection.NorthWest,
            HexDirection.SouthWest,

            HexDirection.SouthEast,
            HexDirection.NorthEast,
            HexDirection.North,
            HexDirection.NorthWest,
            HexDirection.SouthWest,
            HexDirection.South,

            HexDirection.NorthEast,
            HexDirection.North,
            HexDirection.NorthWest,
            HexDirection.SouthWest,
            HexDirection.South,
            HexDirection.SouthEast,
        };

        Assert.AreEqual(36, expected.Length);

        int count = 0;

        for (int i = 0; i < directions.Length; i++) {
            for (int j = 0; j < directions.Length; j++) {
                var dir = directions[i];
                var rot = directions[j];
                var exp = expected[count];

                Assert.AreEqual(exp, HexGrid.RotateDirection(dir, rot));
                count++;
            }
        }

        Assert.AreEqual(36, count);
    }

    [Test]
    public void RingTest() {
        HexCoord pos = new HexCoord(-5, 2, 3);

        // radius 0
        {
            HexCoord[] expected = new HexCoord[] {
                new HexCoord(-5, 2, 3)
            };

            int count = 0;

            foreach (var offset in HexGrid.EnumerateRing(pos, 0)) {
                Assert.Less(count, expected.Length);
                Assert.AreEqual(expected[count], offset);
                count++;
            }

            Assert.AreEqual(expected.Length, count);
        }

        // radius 1
        {
            HexCoord[] expected = new HexCoord[] {
                new HexCoord(-5,  1,  4),
                new HexCoord(-6,  2,  4),
                new HexCoord(-6,  3,  3),
                new HexCoord(-5,  3,  2),
                new HexCoord(-4,  2,  2),
                new HexCoord(-4,  1,  3)
            };

            int count = 0;

            foreach (var offset in HexGrid.EnumerateRing(pos, 1)) {
                Assert.Less(count, expected.Length);
                Assert.AreEqual(expected[count], offset);
                count++;
            }

            Assert.AreEqual(expected.Length, count);
        }

        // radius 2
        {
            HexCoord[] expected = new HexCoord[] {
                new HexCoord(-5, 0, 5),
                new HexCoord(-6, 1, 5),
                new HexCoord(-7, 2, 5),
                new HexCoord(-7, 3, 4),
                new HexCoord(-7, 4, 3),
                new HexCoord(-6, 4, 2),
                new HexCoord(-5, 4, 1),
                new HexCoord(-4, 3, 1),
                new HexCoord(-3, 2, 1),
                new HexCoord(-3, 1, 2),
                new HexCoord(-3, 0, 3),
                new HexCoord(-4, 0, 4)
            };

            int count = 0;

            foreach (var offset in HexGrid.EnumerateRing(pos, 2)) {
                Assert.Less(count, expected.Length);
                Assert.AreEqual(expected[count], offset);
                count++;
            }

            Assert.AreEqual(expected.Length, count);
        }

        // radius 3
        {
            HexCoord[] expected = new HexCoord[] {
                new HexCoord(-5, -1,  6),
                new HexCoord(-6,  0,  6),
                new HexCoord(-7,  1,  6),
                new HexCoord(-8,  2,  6),
                new HexCoord(-8,  3,  5),
                new HexCoord(-8,  4,  4),
                new HexCoord(-8,  5,  3),
                new HexCoord(-7,  5,  2),
                new HexCoord(-6,  5,  1),
                new HexCoord(-5,  5,  0),
                new HexCoord(-4,  4,  0),
                new HexCoord(-3,  3,  0),
                new HexCoord(-2,  2,  0),
                new HexCoord(-2,  1,  1),
                new HexCoord(-2,  0,  2),
                new HexCoord(-2, -1,  3),
                new HexCoord(-3, -1,  4),
                new HexCoord(-4, -1,  5)
            };

            int count = 0;

            foreach (var offset in HexGrid.EnumerateRing(pos, 3)) {
                Assert.Less(count, expected.Length);
                Assert.AreEqual(expected[count], offset);
                count++;
            }

            Assert.AreEqual(expected.Length, count);
        }
    }
}
