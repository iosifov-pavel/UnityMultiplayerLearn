using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] int resourcesPerInterval = 10;
    [SerializeField] float interval = 2;
    float timer = 0f;
    MyNetworkPlayer player;

    [ServerCallback]
    private void Update() {
        timer+=Time.deltaTime;
        if(timer>interval){
            timer=0;
            player.GainResources(resourcesPerInterval);
        }
    }

    public override void OnStartServer()
    {
        player = connectionToClient.identity.gameObject.GetComponent<MyNetworkPlayer>();
        health.ServerOnDie += ServerHandleDie;
        GameStatesHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameStatesHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}
