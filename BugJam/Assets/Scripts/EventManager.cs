using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    public static EventManager instance;

    void Awake() {
        instance = this;
    }

    public event Action onGameEnded;
    public void GameEnded() {
        onGameEnded?.Invoke();
    }
    
    public event Action<Unit> onUnitCreated;
    public void UnitCreated(Unit unit) {
        onUnitCreated?.Invoke(unit);
    }

    public event Action<Unit> onUnitDestroyed;
    public void UnitDestroyed(Unit unit) {
        onUnitDestroyed?.Invoke(unit);
    }
}