using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameSceneUIManager : MonoBehaviour {
    [SerializeField] private Animator pauseDialog;
    [SerializeField] private Animator gameoverDialog;
    [SerializeField] private Text gameoverDialogScoreText;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject newHighscoreMark;

    public static GameSceneUIManager UIManager;
    private GameCoordinator gameCoordinator;


    private void Awake() {
        UIManager = this;
    }

    private void Start() {
        if (pauseDialog == null) Debug.LogError(gameObject.name + ": pause dialog not assigned.", gameObject);
        if (gameoverDialog == null) Debug.LogError(gameObject.name + ": gameover dialog not assigned.", gameObject);
        if (gameoverDialogScoreText == null)
            Debug.LogError(gameObject.name + ": gameover dialog score text not assigned.", gameObject);
        if (scoreText == null) Debug.LogError(gameObject.name + ": score text not assigned.", gameObject);
        gameCoordinator = GameCoordinator.Coordinator;
        if (gameCoordinator == null)
            Debug.LogError(gameObject.name + ": couldn't get GameCoordinator reference.", gameObject);
    }

    private void Update() {
        scoreText.text = "S C O R E : " + gameCoordinator.GetScore().ToString();
    }

    public void ShowGameoverDialog(bool isNewHighscore) {
        newHighscoreMark.SetActive(isNewHighscore);
        gameoverDialogScoreText.text = gameCoordinator.GetScore().ToString();
        gameoverDialog.SetTrigger("appear");
    }

    public void Exit() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowPauseDialog() {
        pauseDialog.SetBool("isHidden", false);
    }

    public void HidePauseDialog() {
        pauseDialog.SetBool("isHidden", true);
    }
}