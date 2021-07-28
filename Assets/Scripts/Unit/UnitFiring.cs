using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] Fighter fighter = null;
    [SerializeField] Projectile bulletPrefab = null;
    [SerializeField] Transform shootPoint = null;
    [SerializeField] float attackRange = 7f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float rotationSpeed = 35f;

    float lastFireTime = Mathf.Infinity;
    Quaternion targetRotation;
    [ServerCallback]
    void Update(){
        if(!CanFire()){return;}
        targetRotation = Quaternion.LookRotation(fighter.GetTarget().transform.position-transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,rotationSpeed*Time.deltaTime);
        lastFireTime+=Time.deltaTime;
        if(lastFireTime<fireRate){return;}
        Fire();
    }
    [Server]
    private bool CanFire(){
        if(fighter.GetTarget()==null){return false;}
        if((fighter.GetTarget().transform.position-transform.position).sqrMagnitude
        >attackRange*attackRange){return false;}
        return true;

    }
    [Server]
    void Fire(){
        lastFireTime=0;
        Projectile newBullet = Instantiate(bulletPrefab,shootPoint.position,shootPoint.rotation);
        NetworkServer.Spawn(newBullet.gameObject, connectionToClient);
    }
}
