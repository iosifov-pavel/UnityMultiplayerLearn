using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab = null;
    [SerializeField] Transform unitSpawnPoint = null;

    
    #region Server
    [Command]
    void CmdSpawnUnit(){
        GameObject unitInstance = Instantiate(
            unitPrefab,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient); 
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
