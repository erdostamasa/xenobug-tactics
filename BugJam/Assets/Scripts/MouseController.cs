using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    [SerializeField] LayerMask tileLayer;
    [SerializeField] Material originMaterial;
    [SerializeField] Material targetMaterial;

    Tile tileUnderMouse;

    Tile originTile;
    Tile targetTile;

    void Update() {
        UpdateMouseInput();
        if (Input.GetMouseButtonDown(0)) {
            if (originTile == null && tileUnderMouse != null) {
                originTile = tileUnderMouse;
                originTile.Select(originMaterial);
            }
            else if (originTile != null && targetTile == null && tileUnderMouse != null) {
                targetTile = tileUnderMouse;
                targetTile.Select(targetMaterial);
                Grid.instance.ShowPath(originTile, targetTile);
            }
            else {
                if (originTile != null) {
                    originTile.Unselect();
                    originTile = null;
                }

                if (targetTile != null) {
                    targetTile.Unselect();
                    targetTile = null;
                }
            }
        }
    }

    void UpdateMouseInput() {
        Vector3 mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 10f, Color.red);
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