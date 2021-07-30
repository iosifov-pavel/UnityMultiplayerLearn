using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class NetworkManagerMy : NetworkManager
{
    [SerializeField] GameObject playerBase = null;
    [SerializeField] GameStatesHandler handler = null;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress= false;

    public List<MyNetworkPlayer> Players {get;} = new List<MyNetworkPlayer>();

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(!isGameInProgress){return;}
        conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        Players.Remove(player);
    }

    public override void OnStopServer()
    {
        Players.Clear();
        isGameInProgress = false;
    }

    public void StartGame(){
        if(Players.Count<2){return;}
        isGameInProgress = true;
        ServerChangeScene("Scene_Map1");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Players.Clear();
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        Players.Add(player);
        player.SetDisplayName($"Player{numPlayers}");
        Vector3 newColor = GenerateColors();
        player.SetPlayerColor(new Color(newColor.x,newColor.y,newColor.z,1));
        Debug.Log(this.numPlayers);
    }

    Vector3 GenerateColors(){
        return new Vector3(Random.Range(0, 1f),Random.Range(0, 1f),Random.Range(0, 1f));
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map")){
            GameStatesHandler instance = Instantiate(handler);
            NetworkServer.Spawn(instance.gameObject);
            foreach(MyNetworkPlayer player in Players){
                GameObject playerSpawner = Instantiate(playerBase,
                GetStartPosition().position, 
                Quaternion.identity); 
                NetworkServer.Spawn(playerSpawner, player.connectionToClient); 
            }
        }
    }
}
