using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] Unit unitPrefab = null;
    [SerializeField] Transform unitSpawnPoint = null;
    [SerializeField] Health health = null;
    [SerializeField] TMP_Text remainigUnitText= null;
    [SerializeField] Image unitProgress = null;
    [SerializeField] int maxUnitQueue = 5;
    [SerializeField] float spawnMoveRange = 8;
    [SerializeField] float unitSpawnDuration = 5;

    [SyncVar(hook =nameof(ClientHandleQueuedUnits))]int queuedUnits;
    [SyncVar]float unitTimer;

    private void Update() {
        if(isServer){
            ProduceUnits();
        }
        if(isClient){
            UpdateTimer();
        }
    }


    #region Server
    [Server]
    private void ProduceUnits()
    {
        if(queuedUnits==0){return;}
        unitTimer+=Time.deltaTime;
        if(unitTimer<unitSpawnDuration){return;}
        Unit unitInstance = Instantiate(
            unitPrefab,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        unitInstance.GetComponent<UnitMovement>().ServerMove(spawnOffset);
        queuedUnits--;
        unitTimer=0;
    }
    [Command]
    void CmdSpawnUnit(){
        if(queuedUnits==maxUnitQueue){return;}
        MyNetworkPlayer player = connectionToClient.identity.GetComponent<MyNetworkPlayer>();
        if(player.GetResources()<unitPrefab.GetResourcesCost()){return;}
        queuedUnits++;
        player.GainResources(-unitPrefab.GetResourcesCost());
    }

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server] private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client    
    [Client]
    private void UpdateTimer()
    {
        float newProgress = unitTimer / unitSpawnDuration;
        unitProgress.fillAmount = newProgress;
        if(newProgress>=1){newProgress=0;}
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!hasAuthority){return;}
        if(eventData.button != PointerEventData.InputButton.Left){return;}
        CmdSpawnUnit();
    }

    void ClientHandleQueuedUnits(int oldValue, int newValue){
        remainigUnitText.text = newValue.ToString();
    }
    #endregion
}
