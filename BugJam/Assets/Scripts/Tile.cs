using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] Transform moveQuad;
    [SerializeField] Transform attackQuad;
    [SerializeField] Material defaultMaterial;
    public Transform unitPosition;

    public int x;
    public int y;
    public bool selectable;
    public bool walkable;
    public List<Tile> neighbours;
    public Unit unit;

    public void DisplayAttack() {
        attackQuad.gameObject.SetActive(true);
    }

    public void DisplayMove() {
        moveQuad.gameObject.SetActive(true);
    }

    public void ClearDisplays() {
        moveQuad.gameObject.SetActive(false);
        attackQuad.gameObject.SetActive(false);
    }
}