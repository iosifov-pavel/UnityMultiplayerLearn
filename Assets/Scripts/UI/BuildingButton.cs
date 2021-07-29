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
    [ClientCallback]
    private void Update() {
        if(player==null){
            NetworkConnection conn = NetworkClient.connection;
            NetworkIdentity id = conn.identity;
            player = id.GetComponent<MyNetworkPlayer>();
        }
    }

    public void StartBuildingOnScene(){
        buildingPreview = Instantiate(building.GetBuildingView());
        buildingPreview.SetActive(false);
        buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();
        StartCoroutine(TryPlaceBuilding(buildingPreview));
    }

    IEnumerator TryPlaceBuilding(GameObject buildingPreview){
        yield return new WaitUntil(()=>!Mouse.current.leftButton.isPressed);
        while(!Mouse.current.rightButton.wasPressedThisFrame){
            Ray cameraRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(cameraRay,out RaycastHit hit,1000,floorMask)){
                if(!buildingPreview.activeSelf){buildingPreview.SetActive(true);}
                buildingPreview.transform.position = hit.point + building.GetViewOffset();
                if(Mouse.current.leftButton.wasPressedThisFrame){
                    player.CmdSpawnBuilding(building.GetId(),hit.point);
                    Destroy(buildingPreview);
                    yield break;
                }
            }
            yield return null;
        }
        Destroy(buildingPreview);
    }
}
