using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {
    public enum GameState {
        Menu,
        Idle,
        SelectMove,
        PlayManeuvers,
        PlayResults,
        GameOver
    }

    GameBoard gameBoard;
    GameState state;

    ManeuverData playerManeuver;
    ManeuverData opponentManeuver;

    public event Action<GameState> OnStateChanged = delegate { };

    void Start() {
        gameBoard = GetComponent<GameBoard>();

        GoToMainMenu();
    }

    void Update() {
        switch (state) {
            case GameState.PlayManeuvers:
                UpdatePlayManeuvers();
                break;
            default:
                break;
        }
    }

    public void GoToState(GameState value) {
        state = value;

        OnStateChanged(value);
    }

    public void GoToMainMenu() {
        ClearGameState();
        GoToState(GameState.Menu);
    }

    void ClearGameState() {
        gameBoard.ClearGameState();

        playerManeuver = null;
        opponentManeuver = null;
    }

    public void SetPlanes(GameObject playerPrefab, GameObject opponentPrefab) {
        gameBoard.SetPlanes(playerPrefab, opponentPrefab);
        GoToState(GameState.Idle);
    }

    public void SetPlayerMove(ManeuverData playerMove) {
        playerManeuver = playerMove;
    }

    public void SetOpponentMove(ManeuverData opponentMove) {
        opponentManeuver = opponentMove;
    }

    public void PlayManeuvers() {
        gameBoard.PlayManeuvers(playerManeuver, opponentManeuver);
        GoToState(GameState.PlayManeuvers);
    }

    void UpdatePlayManeuvers() {
        if (!gameBoard.PlayingManeuver) {
            GoToState(GameState.PlayResults);
        }
    }
}
