using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
abstract public class Weapon : NetworkBehaviour
{
    // [SyncVar]public NetworkIdentity parentNetId; 
    [SerializeField] protected SphereCollider attackRange;
    [SerializeField] public GameObject weaponMuzzle;

    public Animator unitAnimator = null;//devrait etre en protected mais a ce point j'ai plus le temp de faire du beau code



    public float getAttackRange()
    {
        return attackRange.radius;
    }

    //[Server]
    abstract public void Shoot(Unit unit);

    //[Server]
    abstract public void Shoot(Vector3 position);
    //[Command]
    abstract public void CmdShoot(Vector3 position);

}
