using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour {
    [SerializeField] Transform barPrefab;
    Dictionary<Unit, Bar> bars = new Dictionary<Unit, Bar>();

    void Awake() {
        EventManager.instance.onUnitCreated += AddBar;
        EventManager.instance.onUnitDestroyed += RemoveBar;
    }

    void AddBar(Unit unit) {
        if (!bars.ContainsKey(unit)) {
            Transform barObject = Instantiate(barPrefab, transform);
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