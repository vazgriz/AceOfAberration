using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class GameBoardTest {
    [Test]
    public void TestEncodeDecode() {
        HexCoord[] coords = new HexCoord[] {
            // origin
            new HexCoord( 0,  0,  0),

            // Ring 1
            new HexCoord( 0, -1,  1),
            new HexCoord(-1,  0,  1),
            new HexCoord(-1,  1,  0),
            new HexCoord( 0,  1, -1),
            new HexCoord( 1,  0, -1),
            new HexCoord( 1, -1,  0),

            // Ring 2
            new HexCoord( 0, -2,  2),
            new HexCoord(-1, -1,  2),
            new HexCoord(-2,  0,  2),
            new HexCoord(-2,  1,  1),
            new HexCoord(-2,  2,  0),
            new HexCoord(-1,  2, -1),
            new HexCoord( 0,  2, -2),
            new HexCoord( 1,  1, -2),
            new HexCoord( 2,  0, -2),
            new HexCoord( 2, -1, -1),
            new HexCoord( 2, -2,  0),
            new HexCoord( 1, -2,  1),

            // Ring 3
            new HexCoord( 0, -3,  3),
            new HexCoord(-1, -2,  3),
            new HexCoord(-2, -1,  3),
            new HexCoord(-3,  0,  3),
            new HexCoord(-3,  1,  2),
            new HexCoord(-3,  2,  1),

            new HexCoord(-3,  3,  0),
            new HexCoord(-2,  3, -1),
            new HexCoord(-1,  3, -2),
            new HexCoord( 0,  3, -3),
            new HexCoord( 1,  2, -3),
            new HexCoord( 2,  1, -3),

            new HexCoord( 3,  0, -3),
            new HexCoord( 3, -1, -2),
            new HexCoord( 3, -2, -1),
            new HexCoord( 3, -3,  0),
            new HexCoord( 2, -3,  1),
            new HexCoord( 1, -3,  2),
        };

        for (int i = 0; i < coords.Length; i++) {
            HexCoord expectedCoord = coords[i];

            PlaneState state = new PlaneState();
            state.position = expectedCoord;

            EncodedManeuver encoded = GameBoard.EncodeManeuver(state);

            PlaneState decoded = GameBoard.DecodeManeuver(encoded);

            Assert.AreEqual(expectedCoord, decoded.position);
        }
    }

    [Test]
    public void LinearizeTest() {
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 6; y++) {
                for (int z = 0; z < 37; z++) {
                    EncodedManeuver encoded = new EncodedManeuver();
                    encoded.speed = x;
                    encoded.direction = (HexDirection)y;
                    encoded.posIndex = z;

                    int linear = GameBoard.Linearize(encoded);
                    GameBoard.Delinearize(out var encoded2, linear);

                    Assert.AreEqual(encoded.speed, encoded2.speed);
                    Assert.AreEqual(encoded.direction, encoded2.direction);
                    Assert.AreEqual(encoded.posIndex, encoded2.posIndex);
                }
            }
        }
    }

    [Test]
    public void ParseTest() {
        for (int x = 0; x < 3; x++) {
            for (int y = 0; y < 6; y++) {
                for (int z = 0; z < 37; z++) {
                    EncodedManeuver encoded = new EncodedManeuver();
                    encoded.speed = x;
                    encoded.direction = (HexDirection)y;
                    encoded.posIndex = z;

                    string text = GameBoard.PrintManeuver(encoded);
                    EncodedManeuver? encoded2 = GameBoard.ParseManeuver(text);

                    Assert.IsTrue(encoded2.HasValue);
                    Assert.AreEqual(encoded.speed,      encoded2.Value.speed);
                    Assert.AreEqual(encoded.direction,  encoded2.Value.direction);
                    Assert.AreEqual(encoded.posIndex,   encoded2.Value.posIndex);
                }
            }
        }
    }
}
