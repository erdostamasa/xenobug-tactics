using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Storage {
    // 1 = completed
    // 0 = not completed
    public static void LevelCompleted(string levelName) {
        string key = $"level{levelName}completed";
        PlayerPrefs.SetInt(key, 1);
    }

    public static bool IsLevelCompleted(int levelName) {
        string key = $"level{levelName}completed";
        return PlayerPrefs.GetInt(key) == 1;
    }
}