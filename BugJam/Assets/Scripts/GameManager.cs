using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameState state;
    EnemyAI opponent;
    [SerializeField] PlayerController player;

    [SerializeField] TurnDisplay turnDisplay;

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

        EventManager.instance.onUnitDestroyed += RemoveUnit;

        turnDisplay.PlayerTurn();

        player.units.Add(PlaceUnit(3, 0, Unit.Owner.PLAYER));
        player.units.Add(PlaceUnit(4, 0, Unit.Owner.PLAYER));
        player.units.Add(PlaceUnit(5, 0, Unit.Owner.PLAYER));

        opponent.units.Add(PlaceUnit(0, 6, Unit.Owner.ENEMY));
        opponent.units.Add(PlaceUnit(1, 6, Unit.Owner.ENEMY));
        opponent.units.Add(PlaceUnit(2, 6, Unit.Owner.ENEMY));
        opponent.units.Add(PlaceUnit(3, 6, Unit.Owner.ENEMY));
        opponent.units.Add(PlaceUnit(4, 6, Unit.Owner.ENEMY));
        opponent.units.Add(PlaceUnit(5, 6, Unit.Owner.ENEMY));


        StartCoroutine(GameLoop());
    }

    void RemoveUnit(Unit u) {
        opponent.RemoveUnit(u);
    }

    public void Reset() {
        state = GameState.PLAYER_TURN;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GameLoop() {
        while (true) {
            if (state == GameState.ENEMY_TURN) {
                turnDisplay.EnemyTurn();
                yield return new WaitForSeconds(0.5f);

                foreach (Unit opponentUnit in opponent.units) {
                    opponentUnit.SetAvailable();
                }
                yield return StartCoroutine(opponent.MakeMove());

                state = GameState.PLAYER_TURN;
                turnDisplay.PlayerTurn();
                foreach (Unit unit in player.units) {
                    unit.SetAvailable();
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }


    Unit PlaceUnit(int x, int y, Unit.Owner owner) {
        switch (owner) {
            case Unit.Owner.ENEMY:
                return (Grid.instance.SpawnUnit(x, y, enemyUnit, Unit.Owner.ENEMY));
                break;
            case Unit.Owner.PLAYER:
                return (Grid.instance.SpawnUnit(x, y, playerUnit, Unit.Owner.PLAYER));
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