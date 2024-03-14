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
    PlaneState opponentState;
    ManeuverData opponentManeuver;

    public bool SinglePlayer { get; set; }

    public event Action<GameState> OnStateChanged = delegate { };
    public event Action<Plane> OnPlaneSpawned = delegate { };

    public GameBoard GameBoard {
        get {
            return gameBoard;
        }
    }

    void Start() {
        gameBoard = GetComponent<GameBoard>();
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

        OnPlaneSpawned(gameBoard.PlayerPlane);
    }

    public void StartGame() {
        GoToState(GameState.Idle);
    }

    public bool SetPlayerMove(ManeuverData playerMove, string opponentMoveCode) {
        bool success = gameBoard.ValidateManeuvers(playerMove, opponentMoveCode, out PlaneState opponentState, out ManeuverData opponentMove);

        if (success) {
            playerManeuver = playerMove;
            opponentManeuver = opponentMove;
            this.opponentState = opponentState;

            return true;
        }

        return false;
    }

    public void PlayManeuvers() {
        gameBoard.PlayManeuvers(playerManeuver, opponentState, opponentManeuver);
        GoToState(GameState.PlayManeuvers);
    }

    void UpdatePlayManeuvers() {
        if (!gameBoard.PlayingManeuver) {
            FinishManeuvers();
        }
    }

    void FinishManeuvers() {
        GoToState(GameState.PlayResults);
    }
}
