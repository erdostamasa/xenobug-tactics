using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    [SerializeField] Transform moveQuad;
    [SerializeField] Transform attackQuad;
    [SerializeField] Material defaultMaterial;
    public Transform unitPosition;

    [SerializeField] Color color1;
    [SerializeField] Color color2;

    public int x;
    public int y;
    public bool selectable;
    public bool walkable;
    public List<Tile> neighbours;
    private Unit unit;


    public Unit Unit {
        get => unit;
        set => unit = value;
    }

    void Start() {
        //randomize color
        Material randomMat = new Material(Shader.Find("Standard"));
        randomMat.color = Color.Lerp(color1, color2, Random.Range(0f, 1f));
        randomMat.SetFloat("_Glossiness", 0.05f);
        GetComponent<Renderer>().material = randomMat;
    }

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