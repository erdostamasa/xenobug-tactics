using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDisplay : MonoBehaviour {
    [SerializeField] GameObject wonText;
    [SerializeField] GameObject lostText;
    [SerializeField] GameObject turnButton;
    
    
    public void GameWon() {
        wonText.SetActive(true);
        lostText.SetActive(false);
        turnButton.SetActive(false);
    }

    public void GameLost() {
        wonText.SetActive(false);
        lostText.SetActive(true);
        turnButton.SetActive(false);
    }
    
    

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu() {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}