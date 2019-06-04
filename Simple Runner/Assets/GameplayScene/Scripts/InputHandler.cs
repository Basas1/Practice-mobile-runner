using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public delegate void OnScreenTap();
    public static event OnScreenTap onScreenTapEvent;
    private GameCoordinator gameCoordinator;

    private void Start() {
        gameCoordinator = GameCoordinator.Coordinator;
        if (gameCoordinator == null)
            Debug.LogError(gameObject.name + ": couldn't get GameCoordinator reference.", gameObject);
    }

    private void Update() {
        KeyboardInputHandling();
        MobileDeviceInputHandling();
        SharedInputHandling();
    }

    private void MobileDeviceInputHandling() {
        foreach (Touch touch in Input.touches)
            if (touch.phase == TouchPhase.Began)
                OnTap();
    }

    private void KeyboardInputHandling() {
        if (Input.GetKeyDown(KeyCode.Space)) OnTap();
    }

    private void SharedInputHandling() {
        if (Input.GetKeyDown(KeyCode.Escape)) BackButtonAction();
    }

    private void OnTap() {
        onScreenTapEvent?.Invoke();
    }

    private void BackButtonAction() {
        switch (gameCoordinator.GetCurrentGameState()) {
            case GameStates.gameIsRunning: {
                gameCoordinator.PauseGame();
                break;
            }
            case GameStates.gameIsPaused: {
                gameCoordinator.UnpauseGame();
                break;
            }
            case GameStates.gameIsOver: {
                gameCoordinator.RestartGame();
                break;
            }
        }
    }
}