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
        GameStatesHandler.ClientOnGameOver += ClientHandleGameOver;
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
        if(playerBases.Count>1){return;}
        int winner = playerBases[0].connectionToClient.connectionId;
        RpcGameOver($"Player {winner}");
        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client
    [ClientRpc] void RpcGameOver(string winner){
        ClientOnGameOver?.Invoke(winner);
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled=false;
    }

    private void OnDestroy() {
        GameStatesHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    #endregion
}
