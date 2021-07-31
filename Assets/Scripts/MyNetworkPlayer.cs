using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] List<Building> buildings = new List<Building>();
    [SyncVar(hook =nameof(ClientHandleDisplayName))][SerializeField] string displayName ="Missing Name";
    [SyncVar][SerializeField] Color playerColor = Color.white;
    [SyncVar(hook =nameof(ClientHandleResourcesUpdated))]int resources = 500;
    [SyncVar(hook = nameof(ClientPartyOwnerState))] bool isPartyOwner = false;

    public static event Action ClientInfoUpdate;

    private void ClientPartyOwnerState(bool oldV, bool newV){
        if(!hasAuthority){return;}

    }

    public bool IsPartyOwer(){
        return isPartyOwner;
    }

    [Server] void SetPartyOwner(bool state){
        isPartyOwner = state;
    }

    [Command] public void CmdStartGame(){
        ((NetworkManagerMy)NetworkManager.singleton).StartGame();

    }

    public event Action<int> ClientOnResorcesUpdated;
    List<Unit> ownedUnits = new List<Unit>();
    List<Building> ownedBuildings = new List<Building>();
    public List<Unit> GetUnits(){
        return ownedUnits;
    }
    public List<Building> GetBuildings(){
        return ownedBuildings;
    }
    public int GetResources(){
        return resources;
    }
    public Color GetPlayerColor(){
        return playerColor;
    }

    public string GetName(){
        return displayName;
    }
    #region Server
    // Start is called before the first frame update
    [Server] public void GainResources(int amount){
        if(amount>0){resources+=amount;}
        else{
            resources = Mathf.Max(0,resources+amount);
        }
    }

    [Server]
    public void SetDisplayName(string newName){
        displayName = newName;
    }
    [Server]
    public void SetPlayerColor(Color newColor){
        playerColor = newColor;
    }
    [Command]
    private void CmdSetDisplayName(string newDisplayName){
        if(newDisplayName.Length<3 || newDisplayName.Length>12){
            Debug.Log("Wrong name");
            return;
        }        
        RpcLogNewName(newDisplayName); 
        SetDisplayName(newDisplayName);
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUintSpawn+=ServerOnUnitSpawn;
        Unit.ServerOnUintDespawn += ServerOnUnitDespawn;
        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawn;
        DontDestroyOnLoad(gameObject);
    }
    [Server]
    private void ServerOnUnitDespawn(Unit obj)
    {
        if(obj.connectionToClient.connectionId!=connectionToClient.connectionId){return;}
        ownedUnits.Remove(obj);
    }
    [Server]
    private void ServerOnUnitSpawn(Unit obj)
    {
        if (obj.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        ownedUnits.Add(obj);
    }

    [Server]
    private void ServerHandleBuildingDespawn(Building obj)
    {
        if (obj.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        ownedBuildings.Remove(obj);
    }

    [Server]
    private void ServerHandleBuildingSpawn(Building obj)
    {
        if (obj.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        ownedBuildings.Add(obj);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUintSpawn-=ServerOnUnitSpawn;
        Unit.ServerOnUintDespawn -= ServerOnUnitDespawn;
        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawn;
    }

    [Command] public void CmdSpawnBuilding(int buildingID, Vector3 point){
        foreach(Building building in buildings){
            if(building.GetId()!=buildingID){continue;}
            if(resources<building.GetPrice()){return;}
            GainResources(-building.GetPrice());
            Building newBuilding = Instantiate(building,point,building.transform.rotation);
            NetworkServer.Spawn(newBuilding.gameObject,connectionToClient);
            break;
        }
    }

    
    #endregion
    #region Client
    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResorcesUpdated?.Invoke(newValue);
    }

    private void ClientHandleDisplayName(string oldV, string newV)
    {
        ClientInfoUpdate?.Invoke();
    }

    [ClientRpc]
    private void RpcLogNewName(string newName){
        Debug.Log(newName);
    }

    public override void OnStartAuthority()
    {
        if(NetworkServer.active){return;}
        Unit.ClientOnUintSpawn+= AddUnit;
        Unit.ClientOnUintDespawn += RemoveUnit;
        Building.ClientOnBuildingSpawn += AddBuilding;
        Building.ClientOnBuildingDespawn += RemoveBuilding;
    }

    public override void OnStartClient()
    {
        if(NetworkServer.active){return;}
        ((NetworkManagerMy)NetworkManager.singleton).Players.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        ClientInfoUpdate?.Invoke();
        if(!isClientOnly){return;}
        ((NetworkManagerMy)NetworkManager.singleton).Players.Remove(this);
        if(!hasAuthority){return;}
        Unit.ClientOnUintSpawn-= AddUnit;
        Unit.ClientOnUintDespawn -= RemoveUnit;
        Building.ClientOnBuildingSpawn -= AddBuilding;
        Building.ClientOnBuildingDespawn -= RemoveBuilding;
    }

    void AddUnit(Unit unit){
        ownedUnits.Add(unit);
    }

    void RemoveUnit(Unit unit){
        ownedUnits.Remove(unit);
    }

    void AddBuilding(Building building)
    {
        ownedBuildings.Add(building);
    }

    void RemoveBuilding(Building building)
    {
        ownedBuildings.Remove(building);
    }
    #endregion


}
