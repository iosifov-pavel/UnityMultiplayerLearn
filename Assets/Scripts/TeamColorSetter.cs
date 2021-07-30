using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] Renderer[] renderers = new Renderer[0];
    [SyncVar(hook = nameof(ChangeColor))] Color teamColor = new Color();

    #region Server
    public override void OnStartServer()
    {
        MyNetworkPlayer player = connectionToClient.identity.GetComponent<MyNetworkPlayer>();
        teamColor = player.GetPlayerColor();
    }
    #endregion

    #region Client
    private void ChangeColor(Color oldColor, Color newColor)
    {
        foreach(Renderer render in renderers){
            render.material.SetColor("_BaseColor", newColor);
        }
    }
    #endregion
}
