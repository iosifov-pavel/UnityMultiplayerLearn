using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Fighter : NetworkBehaviour
{
    [SerializeField] private Target target;
    #region  Server
    [Command]
    public void CmdSetTarget(GameObject targetTo){
        if(!targetTo.TryGetComponent<Target>(out Target target)) return;
        this.target = target;
    }

    [Server]
    public void ClearTarget(){
        target=null;
    }
    #endregion
    #region  Client
    #endregion
}
