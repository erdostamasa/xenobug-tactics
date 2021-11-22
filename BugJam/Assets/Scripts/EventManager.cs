using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public static EventManager instance;

    void Awake() {
        instance = this;
    }

    public event Action<Unit> onUnitDestroyed;
    public void UnitDestroyed(Unit unit) {
        onUnitDestroyed?.Invoke(unit);
    }
}