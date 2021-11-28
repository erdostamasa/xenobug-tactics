using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void BackToMenu() {
        MenuManager.openLevelSelector = true;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }
}
