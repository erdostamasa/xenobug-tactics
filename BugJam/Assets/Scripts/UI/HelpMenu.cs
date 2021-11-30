using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour {
    [SerializeField] GameObject menu;


    public void ToggleMenu() {
        menu.SetActive(!menu.activeSelf);
    }
}