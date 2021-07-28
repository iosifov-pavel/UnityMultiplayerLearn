using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] UnitSelection selection=null;
    [SerializeField] LayerMask mask = new LayerMask();
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        selection.GetSelectedUnits();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.rightButton.wasPressedThisFrame){
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray,out RaycastHit hit, 1000,mask)){
                if(hit.collider.TryGetComponent<Target>(out Target target))
                {
                    if(target.hasAuthority){
                        TryMove(hit.point);
                        return;
                    }
                    else{
                        TryTarget(target);
                        return;
                    }
                }
                TryMove(hit.point);
            }
        }
    }

    private void TryTarget(Target target){
        foreach (Unit unit in selection.GetSelectedUnits())
        {
            unit.GetFighter().CmdSetTarget(target.gameObject);
        }
    }


    private void TryMove(Vector3 point){
        foreach(Unit unit in selection.GetSelectedUnits()){
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
