using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Building : NetworkBehaviour
{
    [SerializeField] Sprite icon;
    [SerializeField] int price = 100;
    [SerializeField] int id = -1;

    public static event Action<Building> ServerOnBuildingSpawn;
    public static event Action<Building> ServerOnBuildingDespawn;
    public static event Action<Building> ClientOnBuildingSpawn;
    public static event Action<Building> ClientOnBuildingDespawn;

    public int GetId(){
        return id;
    }

    public Sprite GetIcon(){
        return icon;
    }

    public int GetPrice(){
        return price;
    }

    #region Server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawn?.Invoke(this);
    }
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }
    #endregion

}
