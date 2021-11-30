using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDisplay : MonoBehaviour {
    [SerializeField] GameObject wonText;
    [SerializeField] GameObject lostText;
    [SerializeField] GameObject turnButton;
    [SerializeField] List<GameObject> toDisable;


    public void GameWon() {
        wonText.SetActive(true);
        lostText.SetActive(false);
        Initialize();
    }

    public void GameLost() {
        wonText.SetActive(false);
        lostText.SetActive(true);
        Initialize();
    }

    public void Initialize() {
        turnButton.SetActive(false);
        foreach (GameObject o in toDisable) {
            o.SetActive(false);
        }
    }

    public void SelectNextLevel() {
        LevelsSO mapHolder = Resources.Load<LevelsSO>("LevelsContainer");
        int currentIndex = PlayerPrefs.GetInt("selectedLevel", 0);
        if (currentIndex < mapHolder.levels.Count - 1) {
            PlayerPrefs.SetInt("selectedLevel", currentIndex + 1);
            SceneManager.LoadScene(1);
        }
        else {
            SceneManager.LoadScene(0);
        }
    }


    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu() {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}