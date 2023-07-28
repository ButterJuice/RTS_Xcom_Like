using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ImpactGrenade : Trowable
{
    [SerializeField] private GameObject Explosion;


[Server] 
   private void OnCollisionEnter(Collision other) {
        GameObject explosionCaused = Instantiate(Explosion,this.transform.position,this.transform.rotation);
        NetworkServer.Spawn(explosionCaused);
        Destroy(gameObject);
    }

}
