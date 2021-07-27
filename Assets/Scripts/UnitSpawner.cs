using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab = null;
    [SerializeField] Transform unitSpawnPoint = null;
    [SerializeField] Health health = null;

    
    #region Server
    [Command]
    void CmdSpawnUnit(){
        GameObject unitInstance = Instantiate(
            unitPrefab,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient); 
    }

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server] private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!hasAuthority){return;}
        if(eventData.button != PointerEventData.InputButton.Left){return;}
        CmdSpawnUnit();
    }
    #endregion
}
