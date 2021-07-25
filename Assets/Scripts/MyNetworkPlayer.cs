using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook =nameof(HandleUINameUpdated))][SerializeField] string displayName ="Missing Name"; 
    [SyncVar(hook=nameof(HandleDisplayColourUpdated))][SerializeField] Color playerColor = Color.white;
    [SerializeField] TMP_Text playerNameUI = null;
    [SerializeField] Renderer displayColorRender = null;
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
    private void HandleDisplayColourUpdated(Color oldColor, Color newColor){
        displayColorRender.material.SetColor("_BaseColor", newColor);
    }

    private void HandleUINameUpdated(string oldName, string newName){
        playerNameUI.text = newName;
    }

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
