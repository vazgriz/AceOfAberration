using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {
    public enum GameState {
        Idle,
        SelectMove,
        PlayManeuvers,
        PlayResults,
        GameOver
    }

    GameBoard gameBoard;
    GameState state;

    ManeuverData playerManeuver;

    public event Action<GameState> OnStateChanged = delegate { };

    void Start() {
        gameBoard = GetComponent<GameBoard>();

        GoToState(GameState.Idle);
    }

    public void GoToState(GameState value) {
        state = value;

        OnStateChanged(value);
    }

    public void SelectPlayerMove(ManeuverData playerMove) {
        playerManeuver = playerMove;
    }

    public void SelectMoves(ManeuverData playerMove, ManeuverData opponentMove) {
        GoToState(GameState.PlayManeuvers);
    }
}
