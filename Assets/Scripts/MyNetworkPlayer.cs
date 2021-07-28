using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar][SerializeField] string displayName ="Missing Name"; 
    [SyncVar][SerializeField] Color playerColor = Color.white;
    List<Unit> ownedUnits = new List<Unit>();
    List<Building> ownedBuildings = new List<Building>();
    public List<Unit> GetUnits(){
        return ownedUnits;
    }
    public List<Building> GetBuildings(){
        return ownedBuildings;
    }
    #region Server
    // Start is called before the first frame update
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
    }
    [Server]
    private void ServerOnUnitDespawn(Unit obj)
    {
        throw new NotImplementedException();
    }
    [Server]
    private void ServerOnUnitSpawn(Unit obj)
    {
        throw new NotImplementedException();
    }

    [Server]
    private void ServerHandleBuildingDespawn(Building obj)
    {
        
    }

    [Server]
    private void ServerHandleBuildingSpawn(Building obj)
    {
        
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUintSpawn-=AddUnit;
        Unit.ServerOnUintDespawn -= RemoveUnit;
        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawn;
    }
    #endregion
    #region Client


    [ClientRpc]
    private void RpcLogNewName(string newName){
        Debug.Log(newName);
    }

    public override void OnStartAuthority()
    {
        if(NetworkServer.active){return;}
        Unit.ClientOnUintSpawn+= AddUnit;
        Unit.ClientOnUintDespawn += RemoveUnit;
    }

    public override void OnStopClient()
    {
        if(!isClientOnly || !hasAuthority){return;}
        Unit.ClientOnUintSpawn-= AddUnit;
        Unit.ClientOnUintDespawn -= RemoveUnit;
    }

    void AddUnit(Unit unit){
        ownedUnits.Add(unit);
    }

    void RemoveUnit(Unit unit){
        ownedUnits.Remove(unit);
    }
    #endregion


}
