using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour {
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Fighter fighter=null;
    
    Camera mainCamera;
    // Start is called before the first frame update
    #region Server
    [ServerCallback]
    private void Update() {
        if(!agent.hasPath){return;}
        if(agent.remainingDistance>agent.stoppingDistance){return;}
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position){
        fighter.ClearTarget();
        if(!NavMesh.SamplePosition(position,out NavMeshHit hit,1f, NavMesh.AllAreas)){
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client

    #endregion
}
