using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameStatesHandler : NetworkBehaviour
{
    [SerializeField] List<Base> playerBases = new List<Base>();
    public static event Action<string> ClientOnGameOver;
    public static event Action ServerOnGameOver;

    #region Server
    public override void OnStartServer()
    {
        Base.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        Base.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }



    public override void OnStopServer()
    {
        Base.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        Base.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]void ServerHandleBaseSpawned(Base playerBase){
        playerBases.Add(playerBase);
    }

    [Server] void ServerHandleBaseDespawned(Base playerBase)
    {
        playerBases.Remove(playerBase);
        if(playerBases.Count>1 || playerBases.Count==0){return;}
        string winner = playerBases[0].connectionToClient.identity.GetComponent<MyNetworkPlayer>().GetName();
        RpcGameOver($"{winner}");
        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client
    [ClientRpc] void RpcGameOver(string winner){
        ClientOnGameOver?.Invoke(winner);
    }


    private void OnDestroy() {
    }
    #endregion
}
