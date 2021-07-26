using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar][SerializeField] string displayName ="Missing Name"; 
    [SyncVar][SerializeField] Color playerColor = Color.white;
    #region Server
    // Start is called before the first frame update
    [Server]
    public void SetDisplayName(string newName){
        displayName = newName;
    }
    [Server]
    public void SetPlayerColor(Color newColor){
        playerColor = newColor;
    }
    [Command]
    private void CmdSetDisplayName(string newDisplayName){
        if(newDisplayName.Length<3 || newDisplayName.Length>12){
            Debug.Log("Wrong name");
            return;
        }        
        RpcLogNewName(newDisplayName); 
        SetDisplayName(newDisplayName);
    }
    #endregion
    #region Client

    [ContextMenu("SetMyName")]
    private void SetMyName(){
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcLogNewName(string newName){
        Debug.Log(newName);
    }
    #endregion


}
