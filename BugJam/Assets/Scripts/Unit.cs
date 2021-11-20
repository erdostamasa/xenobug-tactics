using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Tile currentTile;
    public int range = 2;
    public Owner owner;

    public void MoveTo(int x, int y) {
        currentTile.unit = null;
        currentTile = Grid.instance.grid[x, y];
        currentTile.unit = this;
        transform.position = currentTile.unitPosition.position;
    }

    public List<Command> GetAvailableMoves() {
        List<Command> moves = new List<Command>();

        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, range);
        
        foreach (Tile tile in reachable) {
            if (tile.unit == null) {
                moves.Add(new MoveUnitCommand(this, tile.x, tile.y));
            }
        }

        return moves;
    }

    public enum Owner {
        PLAYER,
        ENEMY
    }
}