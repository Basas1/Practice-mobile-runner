using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighscoreData {
    public string name = "";
    public int score = 0;
    public string date = "";

    public HighscoreData(string entryName, int entryScore, string entryDate) {
        name = entryName;
        score = entryScore;
        date = entryDate;
    }
}

[System.Serializable]
public class HighscoresList {
    private List<HighscoreData> highscores;
    private int listLength = 5;

    public HighscoresList() {
        highscores = new List<HighscoreData>(listLength + 1);
    }

    public bool IsHighscore(HighscoreData newScore) {
        if (highscores.Count < listLength) return true;
        for (int i = 0; i < highscores.Count; i++)
            if (highscores[i] == null || highscores[i].score < newScore.score)
                return true;
        return false;
    }

    public void AddHighscore(HighscoreData newHighscore) {
        if (highscores.Count == 0) {
            highscores.Add(newHighscore);
            return;
        }
        highscores.Sort((HighscoreData x, HighscoreData y) => {
            if (x.score > y.score) return -1;
            else return 1;
        });
        bool added = false;
        for (int i = 0; i < highscores.Count; i++)
            if (highscores[i].score < newHighscore.score) {
                highscores.Insert(i, newHighscore);
                added = true;
                break;
            }
        if (highscores.Count < listLength && !added) highscores.Add(newHighscore);
        if (highscores.Count > listLength) highscores.RemoveAt(listLength);
    }

    public List<HighscoreData> GetHighscoreDataList() {
        return highscores;
    }

    public int GetCount() {
        return highscores.Count;
    }
}