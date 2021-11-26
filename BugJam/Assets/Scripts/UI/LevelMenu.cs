using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelMenu : MonoBehaviour {
    
    [SerializeField] Transform levelGrid;
    [SerializeField] Transform buttonPrefab;


    public void SetupLevels() {
        //delete all children
        int count = levelGrid.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(levelGrid.GetChild(i).gameObject);
        }


        //add level buttons
        foreach (TextAsset textAsset in LevelHolder.instance.levels) {
            Transform button = Instantiate(buttonPrefab, levelGrid, true);
            button.GetComponent<LevelButton>().levelName = textAsset.name;
            button.GetComponent<LevelButton>().BindLevel();
            button.GetComponentInChildren<TextMeshProUGUI>().text = textAsset.name;
        }
        
    }
}