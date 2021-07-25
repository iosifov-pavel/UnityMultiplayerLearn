using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkManagerMy : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName($"Player{numPlayers}");
        Vector3 newColor = GenerateColors();
        player.SetPlayerColor(new Color(newColor.x,newColor.y,newColor.z,1));
        Debug.Log(this.numPlayers); 
    }

    Vector3 GenerateColors(){
        return new Vector3(Random.Range(0, 1f),Random.Range(0, 1f),Random.Range(0, 1f));
    }
}
