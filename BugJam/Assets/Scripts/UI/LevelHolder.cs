using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelHolder : MonoBehaviour {
    public List<TextAsset> levels;

    public static LevelHolder instance;

    public string selectedLevelName;
    
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public TextAsset GetLevelText() {
        return levels.Where(l => l.name == selectedLevelName).ToList()[0];
    }
}
