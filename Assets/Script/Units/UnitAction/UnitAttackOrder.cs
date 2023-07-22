using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitAttackOrder : UnitAction
{

    [SerializeField] private Weapon weapon;

    private float weaponAttackRange;
    [SyncVar] private Unit targetedUnit;

    //au moment d'ecrire le code, virtual n'est pas encore une notion tres clair
    protected new virtual void Start()
    {
        base.Start();
        weaponAttackRange = weapon.getAttackRange();
    }

    [Client]
    private void Update()
    {

        if(!isOwned) return;
        if (targetedUnit)
        {

            Vector3 closestPointOfTarget = targetedUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
            //Target in attack range
            if (Vector3.Distance(closestPointOfTarget, gameObject.transform.position) < weaponAttackRange)
            {
                gameObject.GetComponent<UnitMovement>().StopMoving();
                weapon.Shoot(targetedUnit);
            }
            else
            {
                gameObject.GetComponent<UnitMovement>().CmdMove(closestPointOfTarget);
            }
        }
        else return;
    }


 [Command]
    public void AttackTarget(Unit targetUnit)
    {
        this.targetedUnit = targetUnit;

        //if target out of attack range
        //if (Vector3.Distance (targetUnit.transform.position, gameObject.transform.position) > weaponAttackRange)
        Vector3 closestPointOfTarget = targetUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
        if (Vector3.Distance(closestPointOfTarget, gameObject.transform.position) > weaponAttackRange)
        {
            base.unitCommandManager.MoveOrder(closestPointOfTarget);
        }
    }

    [Command]
    public void StopAttack()
    {
        this.targetedUnit = null;
    }
    /*TODO: 
    void AttackMove(){

    }
    */

}
