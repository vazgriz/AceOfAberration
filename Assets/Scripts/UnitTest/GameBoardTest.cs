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
            int expectedIndex = i;

            PlaneState state = new PlaneState();
            state.position = expectedCoord;

            EncodedManeuver encoded = GameBoard.EncodeManeuver(state);

            Assert.AreEqual(expectedIndex, encoded.posIndex);

            PlaneState decoded = GameBoard.DecodeManeuver(encoded);

            Assert.AreEqual(expectedCoord, decoded.position);
        }
    }
}
