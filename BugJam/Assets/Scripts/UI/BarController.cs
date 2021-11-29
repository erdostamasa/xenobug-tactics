using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour {
    [SerializeField] Transform playerBarPrefab;
    [SerializeField] Transform enemyBarPrefab;
    Dictionary<Unit, Bar> bars = new Dictionary<Unit, Bar>();

    void Awake() {
        EventManager.instance.onUnitCreated += AddBar;
        EventManager.instance.onUnitDestroyed += RemoveBar;
    }

    void AddBar(Unit unit) {
        if (!bars.ContainsKey(unit)) {
            Transform barObject = null;
            if (unit.owner == Unit.Owner.PLAYER) {
                barObject = Instantiate(playerBarPrefab, transform);
            }
            else if (unit.owner == Unit.Owner.ENEMY) {
                barObject = Instantiate(enemyBarPrefab, transform);
            }

            barObject.GetComponent<Bar>().target = unit.transform;
            bars.Add(unit, barObject.GetComponent<Bar>());
        }
    }


    void RemoveBar(Unit unit) {
        if (bars.ContainsKey(unit)) {
            Destroy(bars[unit].gameObject);
            bars.Remove(unit);
        }
    }
}