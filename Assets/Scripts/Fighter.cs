using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Fighter : NetworkBehaviour
{
    private Target target;

    public Target GetTarget(){
        return target;
    }
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
}
