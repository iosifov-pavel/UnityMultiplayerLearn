using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerMy : NetworkManager
{
    [SerializeField] GameObject playerBase = null;
    [SerializeField] GameStatesHandler handler = null;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName($"Player{numPlayers}");
        Vector3 newColor = GenerateColors();
        player.SetPlayerColor(new Color(newColor.x,newColor.y,newColor.z,1));
        Debug.Log(this.numPlayers);
        GameObject playerSpawner = Instantiate(playerBase,
        conn.identity.transform.position, 
        conn.identity.transform.rotation); 
        NetworkServer.Spawn(playerSpawner, conn); 
    }

    Vector3 GenerateColors(){
        return new Vector3(Random.Range(0, 1f),Random.Range(0, 1f),Random.Range(0, 1f));
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map")){
            GameStatesHandler instance = Instantiate(handler);
            NetworkServer.Spawn(instance.gameObject);
        }
    }
}
