using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject landingPage = null;
    [SerializeField] GameObject hostLobby = null;
    [SerializeField] GameObject joinPanel = null;
    [SerializeField] TMP_InputField inputField = null;
    [SerializeField] Button joinbutton = null;
    [SerializeField] Button startGameButton = null;

    MyNetworkPlayer player=null;

    private void OnEnable() {
        NetworkManagerMy.ClientOnConnected += HandleClientConected;
        NetworkManagerMy.ClientOnDisconnected += HandleClientDisconected;
    }

    private void OnDisable() {
        NetworkManagerMy.ClientOnConnected -= HandleClientConected;
        NetworkManagerMy.ClientOnDisconnected -= HandleClientDisconected;
    }

    public void HostLobby(){
        landingPage.SetActive(false);
        hostLobby.SetActive(true);
        NetworkManager.singleton.StartHost();
    }

    public void BackToMainScreen(){
        landingPage.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void EnterJoinMenu(){
        landingPage.SetActive(false);
        joinPanel.SetActive(true);
    }
    
    public void Join(){
        string adress = inputField.text;
        NetworkManager.singleton.networkAddress=adress;
        NetworkManager.singleton.StartClient();
        joinbutton.interactable = false;
    }

    void HandleClientConected(){
        joinbutton.interactable = true;
        joinPanel.SetActive(false);
        hostLobby.SetActive(true);
        if(NetworkClient.isHostClient){
            startGameButton.gameObject.SetActive(true);
        }
        else{
            startGameButton.gameObject.SetActive(false);
        }
    }

    void HandleClientDisconected(){
        joinbutton.interactable = true;
    }

    public void LeaveLobby(){
        if(NetworkServer.active && NetworkClient.isConnected){
            NetworkManager.singleton.StopHost();
        }
        else{
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }

    public void StartGame(){
        player = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
        player.CmdStartGame();
    }

}
