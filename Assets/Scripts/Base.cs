using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Base : NetworkBehaviour
{
    [SerializeField] Health health;
    public static event Action<Base> ServerOnBaseSpawned;
    public static event Action<Base> ServerOnBaseDespawned;
    public static event Action<int> ServerOnPlayerDie;
    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    #endregion
}
