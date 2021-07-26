using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    Camera mainCamera;
    List<Unit> selectedUnits = new List<Unit>();

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if(Mouse.current.leftButton.wasPressedThisFrame){
            foreach(Unit selectedUnit in selectedUnits){
                selectedUnit.DeSelect();
            }
            selectedUnits.Clear();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame){
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out RaycastHit hit, 1000, mask)){
            if(hit.collider.TryGetComponent<Unit>(out Unit unit)){
                if(unit.hasAuthority){
                    selectedUnits.Add(unit);
                    foreach(Unit selectedUnit in selectedUnits){
                        selectedUnit.Select();
                    }
                }
            }
        }
    }

    public List<Unit> GetSelectedUnits(){
        return selectedUnits;
    }
}
