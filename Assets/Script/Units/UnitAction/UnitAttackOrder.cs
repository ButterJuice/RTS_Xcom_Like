using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitAttackOrder : UnitAction
{
    /*At the time of writing this the game isn't planned to handle multiple weapon main weapon,
     if there is a need to you can try to transforme this in a list and change the code accordigly
      */
    [SerializeField, Tooltip("This is the weapon used for auto-attacking")] private Weapon mainWeapon;
    [SerializeField, Tooltip("this is the weapon used for ability")] private Weapon abilityWeapon;
    private float weaponAttackRange;
    [SyncVar] private Unit targetedUnit;
    private IEnumerator CR_startShooting;
    private bool CR_startShooting_isRunning = false;//I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool
    private IEnumerator CR_goToTarget;
    private bool CR_goToTarget_isRunning = false; //I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool
    [SerializeField] private LayerMask shootingTargetLayer;//A laymask for all target that can block shot

    //au moment d'ecrire le code, virtual n'est pas encore une notion tres clair
    protected new virtual void Start()
    {
        base.Start();
        weaponAttackRange = mainWeapon.getAttackRange();
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
            Vector3 shotOriginePosition = mainWeapon.weaponMuzzle.transform.position;
            Vector3 closestPointOfTarget = targetedUnit.gameObject.GetComponent<Collider>().ClosestPoint(shotOriginePosition);

            gameObject.GetComponent<Collider>().enabled = false;

            Ray rayToTarget = new Ray(shotOriginePosition, closestPointOfTarget - shotOriginePosition);//The second argument is a delta vector
            Physics.Raycast(rayToTarget, out RaycastHit hit, weaponAttackRange, shootingTargetLayer);
            Debug.DrawRay(shotOriginePosition, hit.point - shotOriginePosition, Color.red, 5.0f);
            //Debug.DrawLine(shotOriginePosition, closestPointOfTarget , Color.yellow, 5.0f);

            gameObject.GetComponent<Collider>().enabled = true;
            //Target in attack range
            if ((Vector3.Distance(closestPointOfTarget, gameObject.transform.position) < weaponAttackRange) && (hit.collider.gameObject == targetedUnit.gameObject))
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
            yield return new WaitForSeconds(0.5f);
        }
    }

    [Client]
    IEnumerator StartShooting()
    {
        while (true)
        {
            if (targetedUnit)//here to prevent on error that is caused if there is no target as it is a coroutine
            {
                mainWeapon.CmdShoot(targetedUnit);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    [Client]
    void UseAbilityShoot(Vector3 position)
    {
        abilityWeapon.CmdShoot(position);
    }
    [Client]
    void UseAbilityShoot(Unit unit)
    {
        abilityWeapon.CmdShoot(unit);
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
    public void CmdAttackTarget(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, gameObject.transform.position) > weaponAttackRange)
        {
            base.unitCommandManager.MoveOrder(targetPosition);
        }
    }
    
    [Command]
    public void CmdUseAbilityTarget(Unit targetUnit)
    {
        abilityWeapon.CmdShoot(targetUnit);
    }
    [Command]
    public void CmdUseAbilityTarget(Vector3 targetPosition)
    {
        abilityWeapon.CmdShoot(targetPosition);
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
