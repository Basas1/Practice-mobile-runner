using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour {
    public static MainMenuUIManager UIManager;

    [SerializeField] private Animator startButton;
    [SerializeField] private Animator highscoresButton;
    [SerializeField] private RectTransform highscoresRect;
    [SerializeField] private Animator settingsButton;
    [SerializeField] private float buttonSlideInAnimationLength = 0.5f;
    [SerializeField] private Animator highscoresDialog;
    [SerializeField] private GameObject resetHighscoresButton;
    private bool highscoresOpened = false;
    private HighscoresList highscoresList;
    [SerializeField] private GameObject noHighscoresTextGameObjects;
    [SerializeField] private GameObject[] highscoresTextGameObjects;

    private void Awake() {
        UIManager = this;
    }

    private void Start() {
        if (startButton == null) Debug.LogError(gameObject.name + ": start button not assigned.", gameObject);
        if (highscoresButton == null) Debug.LogError(gameObject.name + ": highscores button not assigned.", gameObject);
        if (settingsButton == null) Debug.LogError(gameObject.name + ": settings button not assigned.", gameObject);
        if (highscoresDialog == null) Debug.LogError(gameObject.name + ": highscores dialog not assigned.", gameObject);
        if (highscoresRect == null)
            Debug.LogError(gameObject.name + ": highscores rectangle not assigned.", gameObject);
        if (noHighscoresTextGameObjects == null)
            Debug.LogError(gameObject.name + ": highscores text not assigned.", gameObject);
        InitializeHighscoresTable();
        StartCoroutine(SlideButtonsIn());

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (resetHighscoresButton == null)
            Debug.LogError(gameObject.name + ": reset highscores button not assigned.", gameObject);
        resetHighscoresButton.SetActive(true);
#endif
    }

    private void InitializeHighscoresTable() {
        noHighscoresTextGameObjects.SetActive(false);
        foreach (GameObject scoreLine in highscoresTextGameObjects) scoreLine.SetActive(false);
        highscoresList = HighscoreSystem.LoadHighscoresList();
        if (highscoresList == null || highscoresList.GetCount() == 0) {
            noHighscoresTextGameObjects.SetActive(true);
            return;
        }
        int highscoresCount = highscoresList.GetCount() <= highscoresTextGameObjects.Length
            ? highscoresList.GetCount()
            : highscoresTextGameObjects.Length;
        List<HighscoreData> highscores = highscoresList.GetHighscoreDataList();
        for (int i = 0; i < highscoresCount; i++) {
            Text highscoreText = highscoresTextGameObjects[i].GetComponent<Text>();
            highscoreText.text = "" + (i + 1).ToString() + ")\t\t\t" + highscores[i].score + "\t\t\t" +
                                 highscores[i].date;
            highscoresTextGameObjects[i].SetActive(true);
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenHighscores() {
        highscoresDialog.SetBool("isHidden", false);
        highscoresOpened = true;
    }

    public void CloseHighscores() {
        highscoresDialog.SetBool("isHidden", true);
        highscoresOpened = false;
    }

    public void ResetHighscores() {
        HighscoreSystem.ResetHighscores();
        InitializeHighscoresTable();
    }

    private IEnumerator SlideButtonsIn() {
        startButton.SetTrigger("appear");
        yield return new WaitForSeconds(buttonSlideInAnimationLength);
        highscoresButton.SetTrigger("appear");
        yield return new WaitForSeconds(buttonSlideInAnimationLength);
        settingsButton.SetTrigger("appear");
    }

    private void Update() {
        InputHandling();
    }

    private void InputHandling() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (highscoresOpened)
                CloseHighscores();
            else
                Application.Quit();
        }
    }
}