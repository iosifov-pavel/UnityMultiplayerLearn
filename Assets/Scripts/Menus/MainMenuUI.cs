using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Steamworks;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject landingPage = null;
    [SerializeField] GameObject hostLobby = null;
    [SerializeField] GameObject joinPanel = null;
    [SerializeField] TMP_InputField inputField = null;
    [SerializeField] Button joinbutton = null;
    [SerializeField] Button startGameButton = null;
    [SerializeField] TMP_Text[] playersInfo = new TMP_Text[4];
    [SerializeField] bool useSteam = false;

    MyNetworkPlayer player=null;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEnter;
    private void Awake() {
        if(!useSteam){return;}
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequest);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    void OnLobbyCreated(LobbyCreated_t callback){
        if(callback.m_eResult != EResult.k_EResultOK){
            landingPage.SetActive(true);
            return;
        }
        NetworkManager.singleton.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAddress",SteamUser.GetSteamID().ToString());
    }

    void OnGameLobbyJoinRequest(GameLobbyJoinRequested_t callback){
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEnter(LobbyEnter_t callback){
        if(NetworkServer.active){return;}
        string haostAdress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAdress");
        NetworkManager.singleton.networkAddress = haostAdress;
        NetworkManager.singleton.StartClient();
        landingPage.SetActive(false);
        
    }

    private void OnEnable() {
        NetworkManagerMy.ClientOnConnected += HandleClientConected;
        NetworkManagerMy.ClientOnDisconnected += HandleClientDisconected;
        MyNetworkPlayer.ClientInfoUpdate += HandleClientInfoUpdate;
    }

    private void HandleClientInfoUpdate()
    {
        foreach(TMP_Text playerName in playersInfo){
            playerName.text = "Waiting for Player...";
        }
        List<MyNetworkPlayer> players = ((NetworkManagerMy)NetworkManager.singleton).Players;
        for(int i = 0;i<players.Count;i++){
            playersInfo[i].text = players[i].GetName();
        }
        startGameButton.interactable = players.Count >= 2;
    }

    private void OnDisable() {
        NetworkManagerMy.ClientOnConnected -= HandleClientConected;
        NetworkManagerMy.ClientOnDisconnected -= HandleClientDisconected;
        MyNetworkPlayer.ClientInfoUpdate -= HandleClientInfoUpdate;
    }

    public void HostLobby(){
        landingPage.SetActive(false);
        hostLobby.SetActive(true);
        if(useSteam){
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
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
