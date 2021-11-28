using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [SerializeField] LevelMenu _levelMenu;
    [SerializeField] GameObject mainMenu;
    
    public static bool openLevelSelector = false;


    void Start() {
        if (openLevelSelector) {
            openLevelSelector = false;
            mainMenu.SetActive(false);
            _levelMenu.gameObject.SetActive(true);
            OpenLevelSelector();
        }
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenLevelSelector() {
        StartCoroutine(_levelMenu.SetupLevels());
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void LoadLevel(string levelName) {
        LevelHolder.instance.selectedLevelName = levelName;
    }
    
    
}