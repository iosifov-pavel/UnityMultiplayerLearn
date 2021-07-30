using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] UnityEvent onSelected=null;
    [SerializeField] UnityEvent onDeselected=null;
    [SerializeField] UnitMovement movement = null;
    [SerializeField] Fighter fighter = null;
    [SerializeField] Health health = null;
    [SerializeField] int resourcesCost = 10;

    public static event Action<Unit> ServerOnUintSpawn;
    public static event Action<Unit> ServerOnUintDespawn;
    public static event Action<Unit> ClientOnUintSpawn;
    public static event Action<Unit> ClientOnUintDespawn;

    public UnitMovement GetUnitMovement(){
        return movement;
    }

    public int GetResourcesCost(){
        return resourcesCost;
    }

    public Fighter GetFighter(){
        return fighter;
    }

    #region  Client
    public void Select(){
        if(!hasAuthority) {return;}
        onSelected?.Invoke();
    }

    public void DeSelect(){
        if(!hasAuthority) {return;}
        onDeselected?.Invoke();
    }

    public override void OnStartAuthority()
    {
        ClientOnUintSpawn?.Invoke(this);
        DeSelect();
    }

    public override void OnStopClient()
    {
        if(!hasAuthority){return;}
        ClientOnUintDespawn?.Invoke(this);
    }

    
    #endregion

    #region  Server
    public override void OnStartServer()
    {
        ServerOnUintSpawn?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnUintDespawn?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server] private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion
}
