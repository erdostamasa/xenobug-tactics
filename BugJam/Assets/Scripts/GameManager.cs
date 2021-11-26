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
    [SerializeField] EndDisplay _endDisplay;
    
    [Header("Units")]
    public Transform playerHeavyPrefab;
    public Transform playerLightPrefab;
    public Transform enemyUnitPrefab;


    public UnitDescriptor enemy;
    public UnitDescriptor playerHeavy;
    public UnitDescriptor playerLight;

    void Awake() {
        instance = this;
    }

    void Start() {
        SetupUnitTypes();

        Application.targetFrameRate = 60;
        //opponent = new RandomEnemyAI();
        opponent = new SmartAI();

        state = GameState.PLAYER_TURN;

        EventManager.instance.onUnitDestroyed += RemoveUnit;

        turnDisplay.PlayerTurn();

        /*opponent.units.Add(Grid.instance.SpawnUnit(3, 6, enemy));
        opponent.units.Add(Grid.instance.SpawnUnit(2, 6, enemy));
        opponent.units.Add(Grid.instance.SpawnUnit(4, 6, enemy));

        player.units.Add(Grid.instance.SpawnUnit(3, 0, playerHeavy));

        player.units.Add(Grid.instance.SpawnUnit(4, 1, playerLight));*/


        Grid.instance.GenerateGrid();
        StartCoroutine(GameLoop());
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

    void RemoveUnit(Unit u) {
        opponent.RemoveUnit(u);
    }

    public void Reset() {
        state = GameState.PLAYER_TURN;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetupUnitTypes() {
        int[,] pattern = {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        enemy = new UnitDescriptor(enemyUnitPrefab, Unit.Owner.ENEMY, pattern);

        pattern = new[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        playerHeavy = new UnitDescriptor(playerHeavyPrefab, Unit.Owner.PLAYER, pattern);

        pattern = new[,] {
            { 1, 0, 1, 0, 1 },
            { 0, 1, 1, 1, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 1, 1, 1, 0 },
            { 1, 0, 1, 0, 1 },
        };
        playerLight = new UnitDescriptor(playerLightPrefab, Unit.Owner.PLAYER, pattern);
    }

    IEnumerator GameLoop() {
        while (true) {
            if (opponent.units.Count == 0) {
                _endDisplay.gameObject.SetActive(true);
                _endDisplay.GameWon();
                state = GameState.GAME_ENDED;
            }

            if (player.units.Count == 0) {
                _endDisplay.gameObject.SetActive(true);
                _endDisplay.GameLost();
                state = GameState.GAME_ENDED;
            }
            
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
        ENEMY_TURN,
        GAME_ENDED
    }
}