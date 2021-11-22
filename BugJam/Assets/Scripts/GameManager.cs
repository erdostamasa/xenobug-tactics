using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameState state;
    public EnemyAI opponent;
    public PlayerController player;
    [SerializeField] LineRenderer line;

    [SerializeField] TurnDisplay turnDisplay;

    [Header("Units")]
    [SerializeField] Transform playerHeavyPrefab;
    [SerializeField] Transform playerLightPrefab;
    [SerializeField] Transform enemyUnitPrefab;


    UnitDescriptor enemy;
    UnitDescriptor playerHeavy;
    UnitDescriptor playerLight;

    void Awake() {
        instance = this;
    }

    public void DisplayLine(Tile from, Tile to, Color color) {
        if (from != to) {
            //check if selected can reach tile
            List<Tile> path = Grid.instance.GetPath(from, to);

            if (path != null) {
                // draw path 
                line.startColor = color;
                line.endColor = color;
                line.positionCount = path.Count + 1;
                for (int i = 0; i < path.Count; i++) {
                    line.SetPosition(i, path[i].transform.position + new Vector3(0, line.transform.position.y, 0));
                }

                line.SetPosition(path.Count, from.transform.position + new Vector3(0, line.transform.position.y, 0));
            }
        }
    }

    public void StraightLine(Tile from, Tile to, Color color) {
        line.positionCount = 2;
        line.startColor = color;
        line.endColor = color;
        line.SetPosition(0, from.transform.position + new Vector3(0, line.transform.position.y, 0));
        line.SetPosition(1, to.transform.position + new Vector3(0, line.transform.position.y, 0));
        
    }

    public void ResetLine() {
        line.positionCount = 0;
    }

    void Start() {
        SetupUnitTypes();
        Application.targetFrameRate = 60;
        opponent = new RandomEnemyAI();
        state = GameState.PLAYER_TURN;

        EventManager.instance.onUnitDestroyed += RemoveUnit;

        turnDisplay.PlayerTurn();

        opponent.units.Add(Grid.instance.SpawnUnit(3, 6, enemy));
        opponent.units.Add(Grid.instance.SpawnUnit(2, 6, enemy));
        opponent.units.Add(Grid.instance.SpawnUnit(4, 6, enemy));

        player.units.Add(Grid.instance.SpawnUnit(3, 0, playerHeavy));
        
        player.units.Add(Grid.instance.SpawnUnit(4, 1, playerLight));


        StartCoroutine(GameLoop());
    }

    void RemoveUnit(Unit u) {
        opponent.RemoveUnit(u);
    }

    public void Reset() {
        state = GameState.PLAYER_TURN;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetupUnitTypes() {
        int[,] pattern = new int[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        enemy = new UnitDescriptor(enemyUnitPrefab, Unit.Owner.ENEMY, pattern);

        pattern = new int[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        playerHeavy = new UnitDescriptor(playerHeavyPrefab, Unit.Owner.PLAYER, pattern);

        pattern = new int[,] {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 }
        };
        playerLight = new UnitDescriptor(playerLightPrefab, Unit.Owner.PLAYER, pattern);
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


    public enum GameState {
        PLAYER_TURN,
        ENEMY_TURN
    }
}