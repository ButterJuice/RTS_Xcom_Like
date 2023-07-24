using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitAttackOrder : UnitAction
{

    [SerializeField] private Weapon weapon;

    private float weaponAttackRange;
    [SyncVar] private Unit targetedUnit;
    private IEnumerator CR_startShooting;
    private bool CR_startShooting_isRunning = false;//I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool
    private IEnumerator CR_goToTarget;
    private bool CR_goToTarget_isRunning = false; //I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool

    //au moment d'ecrire le code, virtual n'est pas encore une notion tres clair
    protected new virtual void Start()
    {
        base.Start();
        weaponAttackRange = weapon.getAttackRange();
        CR_startShooting = StartShooting();
        CR_goToTarget = GoToTarget();
    }

    #region Client
    [Client]
    private void Update()
    {

        if (!isOwned) return;
        if (targetedUnit)
        {

            Vector3 closestPointOfTarget = targetedUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
            //Target in attack range
            if (Vector3.Distance(closestPointOfTarget, gameObject.transform.position) < weaponAttackRange)
            {
                if (CR_goToTarget_isRunning == true)
                {
                    StopCoroutine(CR_goToTarget);
                    CR_goToTarget_isRunning = false;
                    gameObject.GetComponent<UnitMovement>().CmdStopMoving();
                }
                if (CR_startShooting_isRunning == false)
                {
                    StartCoroutine(CR_startShooting);
                    CR_startShooting_isRunning = true;
                }
            }
            //Target out of attack range
            else
            {
                if (CR_startShooting_isRunning == true)
                    StopCoroutine(CR_startShooting);
                CR_startShooting_isRunning = false;

                if (CR_goToTarget_isRunning == false)
                {
                    StartCoroutine(CR_goToTarget);
                    CR_goToTarget_isRunning = true;
                }
            }
        }
        else return;
    }

    [Client]
    IEnumerator GoToTarget()
    {
        while (true)
        {
            if (targetedUnit)
            {
                Vector3 closestPointOfTarget = targetedUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
                gameObject.GetComponent<UnitMovement>().CmdMove(closestPointOfTarget);
            }
            yield return new WaitForSeconds(2);
        }
    }

    [Client]
    IEnumerator StartShooting()
    {
        while (true)
        {
            if (targetedUnit)//here to prevent on error that is caused if there is no target as it is a coroutine
            {
                weapon.CmdShoot(targetedUnit);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion

    #region Server
    /*
    AttackTarget will chose a target to attack, then go to it
    The actual shooting is called in Update
    */
    [Command]
    public void CmdAttackTarget(Unit targetUnit)
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
    public void CmdStopAttack()
    {
        this.targetedUnit = null;
    }
    /*TODO: 
    void AttackMove(){

    }*/

    #endregion

}
