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
    public List<Unit> GetUnits(){
        return ownedUnits;
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
        Unit.ServerOnUintSpawn+=AddUnit;
        Unit.ServerOnUintDespawn += RemoveUnit;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUintSpawn-=AddUnit;
        Unit.ServerOnUintDespawn -= RemoveUnit;
    }
    #endregion
    #region Client


    [ClientRpc]
    private void RpcLogNewName(string newName){
        Debug.Log(newName);
    }

    public override void OnStartClient()
    {
        if(!isClientOnly){return;}
        Unit.ClientOnUintSpawn+= AddUnit;
        Unit.ClientOnUintDespawn += RemoveUnit;
    }

    public override void OnStopClient()
    {
        if(!isClientOnly){return;}
        Unit.ClientOnUintSpawn-= AddUnit;
        Unit.ClientOnUintDespawn -= RemoveUnit;
    }

    void AddUnit(Unit unit){
        if(isClientOnly){
            if(!hasAuthority){return;}
            ownedUnits.Add(unit);
            return;
        }
        if(unit.connectionToClient.connectionId == connectionToClient.connectionId){
            ownedUnits.Add(unit);
        }
    }

    void RemoveUnit(Unit unit){
        if(isClientOnly){
            if(!hasAuthority){return;}
            ownedUnits.Remove(unit);
            return;
        }
        if(unit.connectionToClient.connectionId == connectionToClient.connectionId){
            if(ownedUnits.Contains(unit)){
                ownedUnits.Remove(unit);
            }
        }
    }
    #endregion


}