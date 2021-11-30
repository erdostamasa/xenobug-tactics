using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int levelIndex;
    [SerializeField] Toggle isCompleted;

    public void BindLevel() {
        GetComponentInChildren<Button>().onClick.AddListener(delegate { PlayerPrefs.SetInt("selectedLevel", levelIndex); });
        GetComponentInChildren<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); });
        if (PlayerPrefs.GetInt(levelIndex + "completed", 0) == 1) {
            isCompleted.isOn = true;
        }
        else {
            isCompleted.isOn = false;
        }
    }
}