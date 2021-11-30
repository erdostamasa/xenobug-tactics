using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int levelIndex;

    public void BindLevel() {
        GetComponent<Button>().onClick.AddListener(delegate { PlayerPrefs.SetInt("selectedLevel", levelIndex); });
        GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); });
    }
}