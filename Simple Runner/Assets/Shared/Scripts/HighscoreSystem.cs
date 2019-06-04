using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class HighscoreSystem {
    private static readonly string Path = Application.persistentDataPath + "/highscores.sss";
    private static readonly string defaultPlayerName = "Player";

    public static bool IsNewHighscore(int score) {
        bool result = false;
        if (File.Exists(Path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path, FileMode.Open);
            HighscoresList highscoresList = formatter.Deserialize(stream) as HighscoresList;
            stream.Close();
            result = highscoresList.IsHighscore(new HighscoreData("tempName", score, "tempDate"));
        } else {
            result = true;
        }
        return result;
    }

    public static void AddHighscoreToList(HighscoreData highscore) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream;
        HighscoresList highscoresList;
        if (File.Exists(Path)) {
            stream = new FileStream(Path, FileMode.Open);
            highscoresList = formatter.Deserialize(stream) as HighscoresList;
            if (highscoresList == null)
                Debug.LogError("Highscore System: Couldn't deserialize highscores list from save file.");
            stream.Close();
        } else {
            highscoresList = new HighscoresList();
        }
        stream = new FileStream(Path, FileMode.Create);
        highscoresList.AddHighscore(highscore);
        formatter.Serialize(stream, highscoresList);
        stream.Close();
    }

    public static HighscoresList LoadHighscoresList() {
        if (!File.Exists(Path)) return null;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path, FileMode.Open);
        HighscoresList highscoresList = formatter.Deserialize(stream) as HighscoresList;
        stream.Close();
        return highscoresList;
    }

    public static void ResetHighscores() {
        if (!File.Exists(Path)) return;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path, FileMode.Create);
        HighscoresList highscoresList = new HighscoresList();
        formatter.Serialize(stream, highscoresList);
        stream.Close();
    }

    public static string GetPlayerName() {
        if (!PlayerPrefs.HasKey("PlayerName")) {
            PlayerPrefs.SetString("PlayerName", defaultPlayerName);
            PlayerPrefs.Save();
            return defaultPlayerName;
        }
        return PlayerPrefs.GetString("PlayerName");
    }

    public static void SetPlayerName(string name) {
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
    }

    public static HighscoreData CreateHighscoreData(int score) {
        HighscoreData highscore = new HighscoreData(GetPlayerName(), score, DateTime.Now.ToString("dd/MM/yyyy"));
        return highscore;
    }
}