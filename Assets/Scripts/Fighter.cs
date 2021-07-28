using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Fighter : NetworkBehaviour
{
    private Target target;

    public Target GetTarget(){
        return target;
    }
    #region  Server
    public override void OnStartServer()
    {
        GameStatesHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    public override void OnStopServer()
    {
        GameStatesHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    [Command]
    public void CmdSetTarget(GameObject targetTo){
        if(!targetTo.TryGetComponent<Target>(out Target target)) return;
        this.target = target;
    }

    [Server]
    public void ClearTarget(){
        target=null;
    }
    #endregion
}
