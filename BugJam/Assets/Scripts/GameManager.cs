using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameState state;
    EnemyAI opponent;
    [SerializeField] PlayerController player;

    [SerializeField] TurnDisplay td;

    [Header("Units")]
    [SerializeField] Transform playerUnit;
    [SerializeField] Transform enemyUnit;

    void Awake() {
        instance = this;
    }

    void Start() {
        Application.targetFrameRate = 60;
        opponent = new RandomEnemyAI();
        state = GameState.PLAYER_TURN;
        td.PlayerTurn();

        player.units.Add(PlaceUnit(2, 2, Unit.Owner.PLAYER));
        opponent.units.Add(PlaceUnit(2, 4, Unit.Owner.ENEMY));

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop() {
        while (true) {
            if (state == GameState.ENEMY_TURN) {
                td.EnemyTurn();
                yield return new WaitForSeconds(1f);
                opponent.MakeMove();
                state = GameState.PLAYER_TURN;
                td.PlayerTurn();
            }

            yield return new WaitForEndOfFrame();
        }
    }


    Unit PlaceUnit(int x, int y, Unit.Owner owner) {
        switch (owner) {
            case Unit.Owner.ENEMY:
                return (Grid.instance.SpawnUnit(x, y, enemyUnit));
                break;
            case Unit.Owner.PLAYER:
                return (Grid.instance.SpawnUnit(x, y, playerUnit));
                break;
            default:
                return null;
        }
    }


    public enum GameState {
        PLAYER_TURN,
        ENEMY_TURN
    }
}