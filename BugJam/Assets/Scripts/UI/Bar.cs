using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour {
    public Transform target;


    [SerializeField] float heightOffset = 1f;
    [SerializeField] TextMeshProUGUI damageDisplay;
    [SerializeField] RectTransform healthBar;
    [SerializeField] Transform gridTransform;
    [SerializeField] Transform darkGridTransform;
    [SerializeField] Transform hpBlock;
    [SerializeField] Transform hpBlockDark;

    [SerializeField] Image attackOrb;
    [SerializeField] float unavailableAlpha = 0.2f;

    [Header("Action Points")]
    [SerializeField] GameObject firstAction;
    [SerializeField] GameObject secondAction;

    Unit unit;

    void Start() {
        unit = target.GetComponent<Unit>();
        SetAttackDisplay();
        SetHealthDisplay(unit.health);
        unit.onHealthChanged += UpdateHealthDisplay;
        unit.onUnitAvailable += UpdateAvailablity;
        unit.onUnitActionPointsChanged += UpdateActionPoints;
    }

    void UpdateActionPoints() {
        if (unit.available) {
            if (unit.movedThisTurn) {
                firstAction.SetActive(true);
                secondAction.SetActive(false);
            }
            else {
                firstAction.SetActive(true);
                secondAction.SetActive(true);
            }
        }
        else {
            firstAction.SetActive(false);
            secondAction.SetActive(false);
        }
    }

    void UpdateAvailablity(bool isOn) {
        if (isOn) {
            var tempColor = attackOrb.color;
            tempColor.a = 1f;
            attackOrb.color = tempColor;
        }
        else {
            var tempColor = attackOrb.color;
            tempColor.a = unavailableAlpha;
            attackOrb.color = tempColor;
        }
    }

    void SetAttackDisplay() {
        damageDisplay.text = unit.damage.ToString();
    }

    void SetHealthDisplay(int hp) {
        //clean grid
        int c = gridTransform.childCount;
        for (int i = 0; i < c; i++) {
            Destroy(gridTransform.GetChild(i).gameObject);
        }

        //fill grid
        float width = (hp * 15) + (hp + 1) * 5;
        for (int i = 0; i < hp; i++) {
            Instantiate(hpBlock, gridTransform);
        }

        for (int i = 0; i < hp; i++) {
            Instantiate(hpBlockDark, darkGridTransform);
        }

        healthBar.sizeDelta = new Vector2(width, 40);
    }

    void UpdateHealthDisplay(int hp) {
        //clean grid
        int c = gridTransform.childCount;
        for (int i = 0; i < c; i++) {
            Destroy(gridTransform.GetChild(i).gameObject);
        }

        //fill grid
        float width = (hp * 15) + (hp + 1) * 5;
        for (int i = 0; i < hp; i++) {
            Instantiate(hpBlock, gridTransform);
        }
    }

    void LateUpdate() {
        transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * unit.uiHeightOffset);
    }
}