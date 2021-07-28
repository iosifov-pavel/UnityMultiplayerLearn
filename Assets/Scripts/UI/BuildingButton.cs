using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] Building building = null;
    [SerializeField] Image icon = null;
    [SerializeField] TMP_Text priceText = null;
    [SerializeField] LayerMask floorMask;
    [SerializeField] Button clickButon = null;

    private Camera mainCamera;
    private MyNetworkPlayer player;
    GameObject buildingPreview;
    Renderer buildingRenderer;

    private void Start() {
        mainCamera = Camera.main;
        icon.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
        clickButon.onClick.AddListener(StartBuildingOnScene);
    }

    private void Update() {
        if(player==null){
            player = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
        }
    }

    public void StartBuildingOnScene(){
        buildingPreview = Instantiate(building.GetBuildingView());
        buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();
        StartCoroutine(TryPlaceBuilding(buildingPreview));
    }

    IEnumerator TryPlaceBuilding(GameObject buildingPreview){
        while(true){
            Ray cameraRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(!Physics.Raycast(cameraRay,out RaycastHit hit,1000,floorMask)) {yield return null;}
            buildingPreview.transform.position = hit.point + building.GetViewOffset();
            yield return null;
        }
    }
}
