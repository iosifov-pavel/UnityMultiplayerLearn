using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour {
    [SerializeField] NavMeshAgent agent;
    Camera mainCamera;
    // Start is called before the first frame update
    #region Server
    [Command]
    public void CmdMove(Vector3 position){
        if(!NavMesh.SamplePosition(position,out NavMeshHit hit,1f, NavMesh.AllAreas)){
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client

    #endregion
}
