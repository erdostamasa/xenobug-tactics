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
    [SerializeField] GameObject pauseMenu;

    [Header("Units")]
    public Transform enemyLightPrefab;
    public Transform enemySniperPrefab;
    public Transform playerLightPrefab;
    public Transform playerHeavyPrefab;


    public UnitDescriptor enemyLight;
    public UnitDescriptor enemySniper;
    public UnitDescriptor playerLight;
    public UnitDescriptor playerHeavy;

    void Awake() {
        instance = this;
    }

    void Start() {
        Time.timeScale = 1;
        SetupUnitTypes();

        Application.targetFrameRate = 60;
        //opponent = new RandomEnemyAI();
        opponent = new SmartAI();

        state = GameState.PLAYER_TURN;

        EventManager.instance.onUnitDestroyed += RemoveUnit;

        turnDisplay.PlayerTurn();

        Grid.instance.GenerateGrid();
        StartCoroutine(GameLoop());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
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
        enemyLight = new UnitDescriptor(enemyLightPrefab, Unit.Owner.ENEMY, pattern);
        
        pattern = new[,] {
            { 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1 },
        };
        enemySniper = new UnitDescriptor(enemySniperPrefab, Unit.Owner.ENEMY, pattern);


        pattern = new[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        playerLight = new UnitDescriptor(playerLightPrefab, Unit.Owner.PLAYER, pattern);

        pattern = new[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 1, 1, 1, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 1, 0, 0 },
        };
        playerHeavy = new UnitDescriptor(playerHeavyPrefab, Unit.Owner.PLAYER, pattern);
        
    }

    IEnumerator GameLoop() {
        while (true) {
            if (opponent.units.Count == 0) {
                _endDisplay.gameObject.SetActive(true);
                _endDisplay.GameWon();
                state = GameState.GAME_ENDED;
                EventManager.instance.GameEnded();
            }

            if (player.units.Count == 0) {
                _endDisplay.gameObject.SetActive(true);
                _endDisplay.GameLost();
                state = GameState.GAME_ENDED;
                EventManager.instance.GameEnded();
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