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


    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu() {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}