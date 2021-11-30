using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour {
    [SerializeField] Transform levelGrid;
    [SerializeField] Transform buttonPrefab;


    public IEnumerator SetupLevels() {
        //delete all children
        int count = levelGrid.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(levelGrid.GetChild(i).gameObject);
        }

        LevelsSO mapHolder = Resources.Load<LevelsSO>("LevelsContainer");

        //add level buttons
        for (int i = 0; i < mapHolder.levels.Count; i++) {
            Transform button = Instantiate(buttonPrefab, levelGrid, true);
            button.GetComponent<LevelButton>().levelIndex = i;
            button.GetComponent<LevelButton>().BindLevel();
            button.GetComponentInChildren<TextMeshProUGUI>().text = mapHolder.levels[i].name;
            button.GetComponent<RectTransform>().localScale = Vector3.one;
        }


        yield return null;
    }
}