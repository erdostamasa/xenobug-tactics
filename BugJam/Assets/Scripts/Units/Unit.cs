using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Tile currentTile;
    public Owner owner;
    public int[,] attackPattern;
    public bool available;
    public bool movedThisTurn;
    // [SerializeField] Material availableMaterial;
    // [SerializeField] Material unavailableMaterial;
    // [SerializeField] TextMeshProUGUI healthDisplay;
    // [SerializeField] TextMeshProUGUI attackDisplay;
    public float uiHeightOffset;

    [Header("Particles")]
    [SerializeField] protected ParticleSystem damagedParticles;
    [SerializeField] List<ParticleSystem> moveParticles;


    [Header("Sounds")]
    [SerializeField] protected SoundDescriptor attackSound;
    [SerializeField] protected SoundDescriptor damagedSound;
    [SerializeField] protected SoundDescriptor deathSound;
    [SerializeField] AudioSource moveAudioSource;


    protected Animator anim;

    public int moveRange = 1;
    public int health;
    public int damage;

    public event Action<int> onHealthChanged;
    public event Action<bool> onUnitAvailable;
    public event Action onUnitActionPointsChanged;

    public void UnitActionPointsChanged() {
        onUnitActionPointsChanged?.Invoke();
    }

    float moveDuration = 0.3f;
    float rotateDuration = 0.2f;


    void Start() {
        available = true;
        SetAvailable();
        // healthDisplay.text = health.ToString();
        // attackDisplay.text = damage.ToString();
        anim = GetComponentInChildren<Animator>();
    }


    public void SetAttackPattern(int[,] pattern) {
        attackPattern = pattern;
        attackPattern = Utils.RotateMatrix90Degrees(attackPattern);
    }

    public void MoveTo(int x, int y) {
        Vector3 start = currentTile.transform.position;

        currentTile.Unit = null;
        currentTile = Grid.instance.grid[x, y];
        currentTile.Unit = this;
        transform.position = currentTile.unitPosition.position;


        Vector3 end = currentTile.transform.position;

        if (start != end) {
            Vector3 dir = (end - start).normalized;
            transform.forward = dir;
        }
    }

    public virtual void MoveAnimate(int x, int y) {
        moveAudioSource.mute = false;
        GameManager.instance.moveInProgress = true;

        foreach (ParticleSystem system in moveParticles) {
            system.Play();
        }

        if (anim != null) {
            anim.SetBool("stopMove", false);
            anim.Play("moving");
        }


        Vector3 start = currentTile.transform.position;
        Tile startTile = currentTile;
        List<Tile> path = Grid.instance.GetPath(startTile, Grid.instance.grid[x, y]);

        currentTile.Unit = null;
        currentTile = Grid.instance.grid[x, y];
        currentTile.Unit = this;
        //transform.position = currentTile.unitPosition.position;

        Sequence moves = DOTween.Sequence();


        Vector3 beforePos = startTile.unitPosition.position;
        if (path.Count > 1) {
            for (int i = path.Count - 1; i > 0; i--) {
                Vector3 dir = (path[i].unitPosition.position - beforePos);

                //moves.Append(DOTween.To(() => transform.forward, x => transform.forward = x, dir.normalized, 1f));
                moves.Append(transform.DORotate(Quaternion.LookRotation(dir, Vector3.up).eulerAngles, rotateDuration));
                moves.Append(transform.DOMove(path[i].unitPosition.position, moveDuration));
                beforePos = path[i].unitPosition.position;
            }
        }

        Vector3 dir2 = (currentTile.unitPosition.position - beforePos);
        Vector3 end = currentTile.transform.position;
        if (start != end) {
            moves.Append(transform.DORotate(Quaternion.LookRotation(dir2, Vector3.up).eulerAngles, rotateDuration));
            moves.Append(transform.DOMove(currentTile.unitPosition.position, moveDuration));
        }

        moves.OnComplete(() => {
            if (anim != null) {
                anim.SetBool("stopMove", true);
            }

            GameManager.instance.moveInProgress = false;
            foreach (ParticleSystem system in moveParticles) {
                system.Stop();
            }

            moveAudioSource.mute = true;
        });
    }


    public void Attack(Unit target) {
        target.TakeDamage(damage);
        SetUnavailable();
    }

    public virtual void AttackAnimate(Unit target) {
        SoundManager.instance.PlaySound(attackSound);
        target.TakeDamage(damage);
        if (anim != null && anim.HasState(0, Animator.StringToHash("attack"))) {
            anim.Play("attack");
        }

        transform.forward = (target.transform.position - transform.position).normalized;
        SetUnavailable();
        onUnitActionPointsChanged?.Invoke();
    }


    public void TakeDamage(int dmg) {
        damagedParticles.Play();
        SoundManager.instance.PlaySound(damagedSound);
        health -= dmg;
        onHealthChanged?.Invoke(health);
        if (health <= 0) {
            DestroySelf();
        }

        // healthDisplay.text = health.ToString();
    }

    public void SetAvailable() {
        available = true;
        movedThisTurn = false;
        onUnitAvailable?.Invoke(true);
        onUnitActionPointsChanged?.Invoke();
        //GetComponentInChildren<Renderer>().material = availableMaterial;
    }

    public void SetUnavailable() {
        available = false;
        onUnitAvailable?.Invoke(false);
        onUnitActionPointsChanged?.Invoke();
        //GetComponentInChildren<Renderer>().material = unavailableMaterial;
    }

    public List<Tile> GetAttackableTiles() {
        List<Tile> attackable = new List<Tile>();

        List<(int, int)> targetCoordinates = Utils.MatrixMask(Grid.instance.grid, (currentTile.x, currentTile.y), attackPattern);

        foreach ((int, int) coordinate in targetCoordinates) {
            Unit u = Grid.instance.grid[coordinate.Item1, coordinate.Item2].Unit;
            if ((u != null && u.owner != owner) || (u == null && Grid.instance.grid[coordinate.Item1, coordinate.Item2].walkable)) {
                // check if target tile is visible
                Vector3 start = currentTile.unitPosition.position;
                Vector3 end = Grid.instance.grid[coordinate.Item1, coordinate.Item2].unitPosition.position;
                float distance = (end - start).magnitude;
                if (Physics.Raycast(start, (end - start).normalized, out var hit, distance, ~LayerMask.NameToLayer("Obstacle"))) {
                    //obstacle detected, attack not possible
                    Debug.DrawLine(start, hit.point, Color.red);
                }
                else {
                    attackable.Add(Grid.instance.grid[coordinate.Item1, coordinate.Item2]);
                    Debug.DrawLine(start, end, Color.green);
                }
            }
        }

        return attackable;
    }

    public List<Tile> GetMovableTiles() {
        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, moveRange);

        List<Tile> toRemove = new List<Tile>();
        foreach (Tile tile in reachable) {
            if (tile.Unit != null) {
                toRemove.Add(tile);
            }
        }

        reachable = reachable.Except(toRemove).ToList();

        return reachable;
    }

    public List<AttackCommand> GetAvailableAttacks() {
        List<AttackCommand> attacks = new List<AttackCommand>();

        foreach (Tile attackableTile in GetAttackableTiles()) {
            if (attackableTile.Unit != null && attackableTile.Unit.owner != owner) {
                attacks.Add(new AttackCommand(this, attackableTile.Unit));
            }
        }

        return attacks;
    }

    public List<MoveUnitCommand> GetAvailableMoves() {
        List<MoveUnitCommand> moves = new List<MoveUnitCommand>();

        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, moveRange);


        foreach (Tile tile in reachable) {
            //Debug.Log(tile);
            // possible moves
            if (tile.Unit == null) {
                moves.Add(new MoveUnitCommand(this, tile.x, tile.y));
            }
        }

        moves.Add(new MoveUnitCommand(this, currentTile.x, currentTile.y));

        return moves;
    }

    public virtual void DestroySelf() {
        EventManager.instance.UnitDestroyed(this);
        Destroy(gameObject);
    }

    public enum Owner {
        PLAYER,
        ENEMY
    }
}