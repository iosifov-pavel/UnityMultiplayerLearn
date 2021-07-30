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
    [SerializeField] LayerMask buildingsMask;
    [SerializeField] float rangeForBuilding = 9f;

    private Camera mainCamera;
    private MyNetworkPlayer player;
    GameObject buildingPreview;
    Renderer buildingRenderer;
    BoxCollider buildingColider;

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
            if(conn==null){return;}
            NetworkIdentity id = conn.identity;
            if(id==null){return;}
            player = id.GetComponent<MyNetworkPlayer>();
        }
    }

    public void StartBuildingOnScene(){
        if(player.GetResources()<building.GetPrice()){return;}
        buildingPreview = Instantiate(building.GetBuildingView());
        buildingColider = building.GetComponent<BoxCollider>();
        buildingPreview.SetActive(false);
        buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();
        StartCoroutine(TryPlaceBuilding(buildingPreview, buildingRenderer));
    }

    bool BuildingsInRange(Vector3 point){
        foreach(Building building in player.GetBuildings()){
            if((point-building.transform.position).sqrMagnitude>rangeForBuilding*rangeForBuilding){
                continue;
            }
            return true;
        }
        return false;
    }

    IEnumerator TryPlaceBuilding(GameObject buildingPreview, Renderer buildingRenderer){
        yield return new WaitUntil(()=>!Mouse.current.leftButton.isPressed);
        while(!Mouse.current.rightButton.wasPressedThisFrame){
            Ray cameraRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(cameraRay,out RaycastHit hit,1000,floorMask)){
                if(!buildingPreview.activeSelf){
                    buildingPreview.SetActive(true);
                }
                Vector3 newPosition = hit.point + building.GetViewOffset();
                buildingPreview.transform.position = newPosition;
                bool overlaping =  Physics.CheckBox(newPosition,buildingColider.size/2,Quaternion.identity,buildingsMask);
                if(!overlaping && BuildingsInRange(hit.point)){
                    buildingRenderer.material.SetColor("_BaseColor",new Color(0,1,0,0.5f));
                    if(Mouse.current.leftButton.wasPressedThisFrame){
                        player.CmdSpawnBuilding(building.GetId(),hit.point);
                        Destroy(buildingPreview);
                        yield break;
                    }
                }
                else{
                    buildingRenderer.material.SetColor("_BaseColor",new Color(1,0,0,0.5f));
                }
            }
            yield return null;
        }
        Destroy(buildingPreview);
    }
}
