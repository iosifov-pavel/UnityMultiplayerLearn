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

    private void Start() {
        player = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
        player.ClientOnResorcesUpdated += ClientHandleResources;
        ClientHandleResources(player.GetResources());
    }

    [ClientCallback]
    private void Update() {
    }

    private void OnDestroy() {
        player.ClientOnResorcesUpdated -= ClientHandleResources;
    }

    private void ClientHandleResources(int obj)
    {
        resourcesText.text = obj.ToString();
    }
}
