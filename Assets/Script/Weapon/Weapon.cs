using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
abstract public class Weapon : NetworkBehaviour
{
    [SerializeField] protected SphereCollider attackRange;
    [SerializeField] protected GameObject weaponMuzzle;


    public float getAttackRange(){          
        return attackRange.radius;
    }

//[Command]
    abstract public void CmdShoot(Unit unit);

}
