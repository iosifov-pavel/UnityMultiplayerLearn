using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] Rigidbody rb = null;
    [SerializeField] float destroyTime = 2f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] int damage = 10;

    private void Start() {
        rb.velocity = transform.forward*projectileSpeed;
    }

    public override void OnStartServer()
    {   
        Invoke(nameof(DestroySelf),destroyTime);
    }

    [Server]
    void DestroySelf(){
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity)){
            if(identity.connectionToClient == connectionToClient) {return;}
        }
        if(other.TryGetComponent<Health>(out Health health)){
            health.TakeDamage(damage);
        }
        DestroySelf();
    }
}
