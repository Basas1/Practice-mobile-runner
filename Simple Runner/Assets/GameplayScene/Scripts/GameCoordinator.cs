using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates {
    gameIsRunning,
    gameIsPaused,
    gameIsOver,
};

public class GameCoordinator : MonoBehaviour {
    public static GameCoordinator Coordinator;
    public delegate void PauseAction();
    public delegate void UnpauseAction();
    public static event PauseAction OnPause;
    public static event UnpauseAction OnUnpause;
    private GameStates gameState;

    [SerializeField] private float startGameSpeed = 7f;
    [SerializeField] private float maxGameSpeed = 10f;
    [SerializeField] private float timeTillMaxSpeed = 60f;
    [SerializeField] private float gameplayTime;
    private float gameSpeed;
    private float currentScore;

    private void Awake() {
        Coordinator = this;
    }

    private void Start() {
        gameSpeed = startGameSpeed;
        gameplayTime = 0f;
        gameState = GameStates.gameIsRunning;
    }

    private void PrintHighscores() {
        HighscoresList hsl = HighscoreSystem.LoadHighscoresList();
        if (hsl == null) {
            print("hsl = null");
            return;
        }
        List<HighscoreData> highscores = hsl.GetHighscoreDataList();
        print("\t --- HIGHSCORES ---");
        for (int i = 0; i < highscores.Count; i++)
            print("\t" + highscores[i].name + "\t --- \t" + highscores[i].score + "\t --- \t" + highscores[i].date);
        print("\t ++++++++++++++++++");
    }

    private void Update() {
        if (gameState != GameStates.gameIsRunning) return;
        gameplayTime += Time.deltaTime;
        if (gameSpeed != maxGameSpeed) UpdateGameSpeed();
        currentScore += Time.deltaTime * gameSpeed;
    }

    private void UpdateGameSpeed() {
        if (gameplayTime >= timeTillMaxSpeed) {
            gameSpeed = maxGameSpeed;
            return;
        }
        gameSpeed = startGameSpeed + (maxGameSpeed - startGameSpeed) * gameplayTime / timeTillMaxSpeed;
    }

    public void GameOver() {
        gameState = GameStates.gameIsOver;
        PauseGameplayObjects();
        bool newHighscore = HighscoreSystem.IsNewHighscore((int)currentScore);
        if (newHighscore) {
            HighscoreSystem.AddHighscoreToList(HighscoreSystem.CreateHighscoreData((int)currentScore));
        }
        GameSceneUIManager.UIManager.ShowGameoverDialog(newHighscore);
    }

    public void RestartGame() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void PauseGameplayObjects() {
        OnPause?.Invoke();
    }

    private void UnpauseGameplayObjects() {
        OnUnpause?.Invoke();
    }

    public void PauseGame() {
        gameState = GameStates.gameIsPaused;
        PauseGameplayObjects();
        GameSceneUIManager.UIManager.ShowPauseDialog();
    }

    public void UnpauseGame() {
        gameState = GameStates.gameIsRunning;
        UnpauseGameplayObjects();
        GameSceneUIManager.UIManager.HidePauseDialog();
    }

    public float GetGameSpeed() {
        return gameSpeed;
    }

    public int GetScore() {
        return (int) currentScore;
    }

    public GameStates GetCurrentGameState() {
        return gameState;
    }

    private void OnGUI() {
        DebugGUI();
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("DEVELOPMENT_BUILD")]
    private void DebugGUI() {
        GUI.Label(new Rect(10, 10, 130, 20), "Game speed = " + gameSpeed.ToString("0.00"));
        GUI.Label(new Rect(200, 10, 100, 20), "Game time = " + (int) gameplayTime);
    }
}