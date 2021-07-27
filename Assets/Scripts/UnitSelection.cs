using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [SerializeField] RectTransform selectionBox = null;
    Camera mainCamera;
    private MyNetworkPlayer player;
    private Vector2 startPosition = Vector2.zero;
    List<Unit> selectedUnits = new List<Unit>();

    private void Start() {
        mainCamera = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
    }

    private void Update() {
        if(player==null){
            player = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
            return;
        }
        if(Mouse.current.leftButton.wasPressedThisFrame){
            StartSelectionArea();
        }
        else if(Mouse.current.leftButton.isPressed){
            UpdateSelectionArea();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame){
            ClearSelectionArea();
        }
    }



    private void StartSelectionArea(){
        if(!Keyboard.current.leftCtrlKey.isPressed){
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.DeSelect();
            }
            selectedUnits.Clear();
        }
        selectionBox.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        selectionBox.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(selectionBox.sizeDelta.magnitude==0){
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, mask))
            {
                if (hit.collider.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit.hasAuthority)
                    {   
                        if(!selectedUnits.Contains(unit)){
                            selectedUnits.Add(unit);
                            foreach (Unit selectedUnit in selectedUnits)
                            {
                                selectedUnit.Select();
                            }
                        }
                    }
                }
            }
        }
        else{
            if(player.GetUnits().Count!=0){
                Vector2 min = selectionBox.anchoredPosition - selectionBox.sizeDelta/2;
                Vector2 max = selectionBox.anchoredPosition + selectionBox.sizeDelta/2;
                foreach(Unit unit in player.GetUnits()){
                    Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                    if(screenPosition.x>min.x
                    && screenPosition.x<max.x
                    && screenPosition.y>min.y
                    && screenPosition.y<max.y){
                        if(unit.hasAuthority){
                            selectedUnits.Add(unit);
                            unit.Select();
                        }
                    }
                }
            }
        }
        selectionBox.gameObject.SetActive(false);
    }

    public List<Unit> GetSelectedUnits(){
        return selectedUnits;
    }
}
