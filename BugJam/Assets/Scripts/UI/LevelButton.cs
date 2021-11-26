using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public string levelName;

    public void BindLevel() {
        GetComponent<Button>().onClick.AddListener(delegate { LevelHolder.instance.selectedLevelName = levelName; });
        GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); });
    }
}