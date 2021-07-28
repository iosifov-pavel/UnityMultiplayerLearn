using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SyncVar(hook=nameof(HandleHealthChange))] int currentHealth;
    public event Action ServerOnDie;
    public event Action<int,int> ClientOnHealthChange;
    #region Server
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        Base.ServerOnPlayerDie += PlayerDieHandler;
    }
    [Server]
    private void PlayerDieHandler(int id)
    {
        if(id!=connectionToClient.connectionId){return;}
        TakeDamage(currentHealth);
    }

    public override void OnStopServer()
    {
        Base.ServerOnPlayerDie -= PlayerDieHandler;
    }

    [Server]
    public void TakeDamage(int damage){
        if(currentHealth==0) {return;}
        currentHealth = Mathf.Max(currentHealth-damage,0);
        if(currentHealth!=0){return;}
        ServerOnDie?.Invoke();
    }
    #endregion

    #region Client
    void HandleHealthChange(int oldHealth, int newHealth){
        ClientOnHealthChange?.Invoke(maxHealth,newHealth);
    }
    #endregion
}
