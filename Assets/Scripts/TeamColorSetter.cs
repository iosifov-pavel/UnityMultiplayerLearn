using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField]List<Renderers> renders = new List<Renderers>();
    [SyncVar(hook = nameof(ChangeColor))] Color teamColor = new Color();

    [System.Serializable]
    public class Renderers{
        [SerializeField] public Renderer renderer = null;
        [SerializeField] public bool[] states = new bool[0];

    }

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
        foreach(Renderers render in renders){
            int matCount = render.renderer.materials.Length;
            for(int i=0;i<matCount;i++){
                if(render.states[i]){
                    render.renderer.materials[i].SetColor("_BaseColor",newColor);
                }
            }
        }
    }
    #endregion
}
