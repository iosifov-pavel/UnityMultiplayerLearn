using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour {
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Fighter fighter=null;
    [SerializeField] float chaseRange = 6f;
    [SerializeField] float stoppingDistance = 20f;
    Camera mainCamera;
    // Start is called before the first frame update
    #region Server
    public override void OnStartServer()
    {
        GameStatesHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    public override void OnStopServer()
    {
        GameStatesHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update() {
        if(fighter.GetTarget()!=null){
            float sqrMagnitude = (fighter.GetTarget().transform.position - transform.position).sqrMagnitude;
            if (sqrMagnitude > chaseRange * chaseRange
            && sqrMagnitude < stoppingDistance * stoppingDistance)
            {
                agent.SetDestination(fighter.GetTarget().transform.position);
            }
            else if(agent.hasPath){
                agent.ResetPath();
            }
            return;
        }
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
