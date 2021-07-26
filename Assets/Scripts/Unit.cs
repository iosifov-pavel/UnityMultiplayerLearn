using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] UnityEvent onSelected=null;
    [SerializeField] UnityEvent onDeselected=null;
    [SerializeField] UnitMovement movement = null;

    public UnitMovement GetUnitMovement(){
        return movement;
    }

    #region  Client
    public void Select(){
        if(!hasAuthority) {return;}
        onSelected?.Invoke();
    }

    public void DeSelect(){
        if(!hasAuthority) {return;}
        onDeselected?.Invoke();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        DeSelect();
    }
    #endregion

    #region  Server

    #endregion
}
