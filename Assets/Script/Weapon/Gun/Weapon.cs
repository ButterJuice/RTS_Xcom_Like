using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
abstract public class Weapon : NetworkBehaviour
{
    // [SyncVar]public NetworkIdentity parentNetId; 
    [SerializeField] protected SphereCollider attackRange;
    [SerializeField] public GameObject weaponMuzzle;


    public float getAttackRange(){          
        return attackRange.radius;
    }

//[Command]
    abstract public void CmdShoot(Unit unit);

//[Command]
    abstract public void CmdShoot(Vector3 position);

}
