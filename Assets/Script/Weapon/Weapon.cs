using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
abstract public class Weapon : NetworkBehaviour
{
    [SerializeField] protected SphereCollider attackRange;


//[Server]
    abstract public float getAttackRange();

//[Server]
    abstract public void Shoot(Unit unit);

}
