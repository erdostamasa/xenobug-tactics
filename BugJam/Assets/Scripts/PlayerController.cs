using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour {
    [SerializeField] LayerMask tileLayer;
    Tile tileUnderMouse;
    public List<Unit> units;


    void Update() {
        if (GameManager.instance.state == GameManager.GameState.ENEMY_TURN) return;

        UpdateMouseInput();
        //move player unit to random position
        if (Input.GetMouseButtonDown(0)) {
            if (tileUnderMouse != null && tileUnderMouse.unit != null && units.Contains(tileUnderMouse.unit)) {
                var moves = tileUnderMouse.unit.GetAvailableMoves();
                int i = Random.Range(0, moves.Count);
                moves[i].Execute();
                GameManager.instance.state = GameManager.GameState.ENEMY_TURN;
            }
        }
    }

    void UpdateMouseInput() {
        Vector3 mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        //Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 10f, Color.red);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 50f, tileLayer)) {
            Tile mouseOverTile = hit.collider.GetComponent<Tile>();
            if (mouseOverTile.selectable) {
                tileUnderMouse = mouseOverTile;
            }
        }
        else {
            tileUnderMouse = null;
        }
    }
}