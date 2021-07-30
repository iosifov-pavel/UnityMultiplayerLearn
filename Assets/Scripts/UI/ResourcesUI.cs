using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] TMP_Text resourcesText = null;
    MyNetworkPlayer player;

    [ClientCallback]
    private void Update() {
        if(player ==null){
            NetworkConnection conn = NetworkClient.connection;
            if(conn==null){return;}
            NetworkIdentity id = conn.identity;
            if(id==null){return;}
            player = id.GetComponent<MyNetworkPlayer>();
            player.ClientOnResorcesUpdated += ClientHandleResources;
            ClientHandleResources(player.GetResources());
        }
    }

    private void OnDestroy() {
        player.ClientOnResorcesUpdated -= ClientHandleResources;
    }

    private void ClientHandleResources(int obj)
    {
        resourcesText.text = obj.ToString();
    }
}
