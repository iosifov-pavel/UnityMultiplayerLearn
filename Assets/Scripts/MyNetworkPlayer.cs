using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar][SerializeField] string displayName ="Missing Name"; 
    [SyncVar][SerializeField] Color playerColor = Color.white;
    // Start is called before the first frame update
    [Server]
    public void SetDisplayName(string newName){
        displayName = newName;
    }

    public void SetPlayerColor(Color newColor){
        playerColor = newColor;
    }
}
