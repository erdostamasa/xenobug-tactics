using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UnitDescriptor {
    public Transform unitPrefab;
    public Unit.Owner owner;
    public int[,] attackPattern;

    public UnitDescriptor(Transform unitPrefab, Unit.Owner owner, int[,] attackPattern) {
        this.unitPrefab = unitPrefab;
        this.owner = owner;
        this.attackPattern = attackPattern;
    }
}