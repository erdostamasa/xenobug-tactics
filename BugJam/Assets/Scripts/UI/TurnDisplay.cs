using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnDisplay : MonoBehaviour {
    [SerializeField] GameObject playerDisplay;
    [SerializeField] GameObject enemyDisplay;
    [SerializeField] GameObject bg;

    public void PlayerTurn() {
        playerDisplay.SetActive(true);
        enemyDisplay.SetActive(false);
        bg.SetActive(false);
    }

    public void EnemyTurn() {
        playerDisplay.SetActive(false);
        enemyDisplay.SetActive(true);
        bg.SetActive(true);
    }
}