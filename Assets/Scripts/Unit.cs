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

    public static event Action<Unit> ServerOnUintSpawn;
    public static event Action<Unit> ServerOnUintDespawn;
    public static event Action<Unit> ClientOnUintSpawn;
    public static event Action<Unit> ClientOnUintDespawn;

    public UnitMovement GetUnitMovement(){
        return movement;
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
        base.OnStartAuthority();
        DeSelect();
    }

    public override void OnStartClient()
    {
        if(!isClientOnly || !hasAuthority){return;}
        ClientOnUintSpawn?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if(!isClientOnly || !hasAuthority){return;}
        ClientOnUintDespawn?.Invoke(this);
    }
    #endregion

    #region  Server
    public override void OnStartServer()
    {
        ServerOnUintSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUintDespawn?.Invoke(this);
    }
    #endregion
}
