using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SyncVar/*(hook =nameof(ChangeHealth))*/] int currentHealth;
    public event Action ServerOnDie;
    #region Server
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void TakeDamage(int damage){
        if(currentHealth==0) {return;}
        currentHealth = Mathf.Max(currentHealth-damage,0);
        if(currentHealth!=0){return;}
        ServerOnDie?.Invoke();
        Debug.Log("I deid");
    }
    #endregion

    #region Client
    #endregion
}
