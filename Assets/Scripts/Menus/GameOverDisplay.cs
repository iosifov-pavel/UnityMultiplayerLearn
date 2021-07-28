using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text winnerText=null;
    [SerializeField] GameObject displayParent = null;
    private void Start() {
        GameStatesHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string obj)
    {
        winnerText.text = $"{obj} Has Won";
        displayParent.SetActive(true);
    }

    public void LeaveGame(){
        if(NetworkServer.active && NetworkClient.isConnected){
            NetworkManager.singleton.StopHost();
        }
        else{
            NetworkManager.singleton.StopClient();
        }
    }

    private void OnDestroy() {
        GameStatesHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
}
