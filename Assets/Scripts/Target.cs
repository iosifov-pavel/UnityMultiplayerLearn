using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform targetPoint=null;
    public Transform GetTargetPoint(){
        return targetPoint;
    }
}
