using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/*
The class UnitAttackOrder handle all action an unit can take related to weapons
it can be the main weapon for auto attack and an ability weapon for abilities
As a reminder a weapon is a game object with a range and a muzzle
*/
public class UnitAttackOrder : UnitAction
{
    /*At the time of writing this, the game isn't planned to handle multiple weapon main weapon.
      */
    [SerializeField] private UnitStats unitStats;

    [SerializeField] private Unit myUnit;
    private Weapon mainWeapon;
    private Weapon abilityWeapon;
    private float weaponAttackRange;
    [SyncVar] private Unit targetedUnit;
    private IEnumerator CR_startShooting;
    private bool CR_startShooting_isRunning = false;//I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool
    private IEnumerator CR_goToTarget;
    private bool CR_goToTarget_isRunning = false; //I did not find a way to  check if a coroutine is running otherwise so instead I am using a bool
    [SerializeField, Tooltip("A laymask for all target that can block shot")] private LayerMask shootingTargetLayer;
    [HideInInspector, SyncVar] private bool ceaseFire = false;//if this is true the unit will stop trying to shoot enemies at range and stop shooting all together (with the main weapon)

    //au moment d'ecrire le code, virtual n'est pas encore une notion tres clair
    protected new virtual void Start()
    {
        base.Start();

    }
    //This function is called inside the UnitStats Start because WeaponInnit need some initialisation inside unitStat first
    public void WeaponInnit()
    {
        mainWeapon = unitStats.mainWeapon;
        abilityWeapon = unitStats.abilityWeapon;

        weaponAttackRange = mainWeapon.getAttackRange();
        if (isServer)
        {
            CR_startShooting = StartShooting();
            CR_goToTarget = GoToTarget();
        }

    }



    #region Client
    private void Update()
    {

        // if (!isOwned) return;
        if (isServer)
        {
            if (targetedUnit & !ceaseFire)
            {
                attackTarget();
            }
            else return;
        }

    }

    /*
    This function is the manager of auto attack, it will make the unit follow the target and shoot it if in range
    */
    [Server]
    void attackTarget()
    {
        Vector3 shotOriginePosition = mainWeapon.weaponMuzzle.transform.position;
        Collider targetedUnitCollider = targetedUnit.gameObject.GetComponent<Collider>();
        Vector3 closestPointOfTarget = targetedUnitCollider.ClosestPoint(shotOriginePosition);

        gameObject.GetComponent<Collider>().enabled = false;

        Ray rayToTarget = new Ray(shotOriginePosition, closestPointOfTarget - shotOriginePosition);//The second argument is a delta vector

        Physics.Raycast(rayToTarget, out RaycastHit hit, weaponAttackRange, shootingTargetLayer);

        Collider hitCollider = hit.collider;
        //When the muzzle is inside the target, then the rayCast doesn't work this is my workArround
        if (!hitCollider)
            if (targetedUnitCollider.bounds.Contains(shotOriginePosition))
                hitCollider = targetedUnitCollider;


        // Debug.DrawRay(shotOriginePosition, hit.point - shotOriginePosition, Color.red, 5.0f);
        // Debug.DrawLine(shotOriginePosition, closestPointOfTarget , Color.yellow, 5.0f);

        gameObject.GetComponent<Collider>().enabled = true;



        // Debug.Log("hit.collider.gameObject = ", hitCollider.gameObject);
        // Debug.Log("targetedUnit.gameObject = ", targetedUnit.gameObject);


        //Target in attack range
        if ((Vector3.Distance(targetedUnit.transform.position, gameObject.transform.position) < weaponAttackRange) && (hitCollider.gameObject == targetedUnit.gameObject))
        {


            if (CR_goToTarget_isRunning == true)
            {
                StopCoroutine(CR_goToTarget);
                CR_goToTarget_isRunning = false;
                gameObject.GetComponent<UnitMovement>().ServerStopMoving();
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


    public void TurnOnCeaseFire()
    {
        ceaseFire = true;
    }
    public void TurnOffCeaseFire()
    {
        ceaseFire = false;
    }
    [Command]
    public void CmdTurnOffCeaseFire()
    {
        TurnOffCeaseFire();
    }

    [Server]
    IEnumerator GoToTarget()
    {
        while (true)
        {
            if (targetedUnit == null)
            {
                break;
            }
            if (targetedUnit)
            {
                Vector3 closestPointOfTarget = targetedUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
                gameObject.GetComponent<UnitMovement>().ServerMove(closestPointOfTarget);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    [Server]
    IEnumerator StartShooting()
    {
        while (true)
        {
            if (targetedUnit & !ceaseFire & gameObject.GetComponent<UnitMovement>().agent.isStopped)//targetedUnit is here to prevent on error that is caused if there is no target as it is a coroutine
            {
                // test();
                mainWeapon.Shoot(targetedUnit);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }


    [Server]
    void UseAbilityShoot(Vector3 position)
    {
        abilityWeapon.Shoot(position);
    }
    [Server]
    void UseAbilityShoot(Unit unit)
    {
        abilityWeapon.Shoot(unit);
    }

    [ClientRpc]
    void Rpc_CR_UseAbility(Vector3 targetPosition)
    {

        StartCoroutine(UseAbility(targetPosition));
    }
    [Client]
    IEnumerator UseAbility(Vector3 targetPosition)
    {
        if (isOwned)
        {
            bool shooted = false;
            while (!shooted)
            {
                if (Vector3.Distance(targetPosition, gameObject.transform.position) <= abilityWeapon.getAttackRange())
                {
                    abilityWeapon.CmdShoot(targetPosition);
                    shooted = true;
                    CmdTurnOffCeaseFire();
                }
                yield return null;
            }

            gameObject.GetComponent<UnitMovement>().ClientStopMoving();
        }
        yield return null;
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


        targetUnit.AddAttackingUnit(myUnit);

        // //if target out of attack range
        // //if (Vector3.Distance (targetUnit.transform.position, gameObject.transform.position) > weaponAttackRange)
        // Vector3 closestPointOfTarget = targetUnit.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);
        // if (Vector3.Distance(closestPointOfTarget, gameObject.transform.position) > weaponAttackRange)
        // {
        //     base.unitCommandManager.RpcMoveOrder(base.unit, closestPointOfTarget);
        // }
    }

    [Command]
    public void cmdStopAttacking()
    {
        serverStopAttacking();
    }
    [Server]
    public void serverStopAttacking()
    {
        // ceaseFire = true;
        targetedUnit = null;
    }
    // [Command]
    // public void CmdAttackTarget(Vector3 targetPosition)
    // {
    //     if (Vector3.Distance(targetPosition, gameObject.transform.position) > weaponAttackRange)
    //     {
    //         base.unitCommandManager.RpcMoveOrder(base.unit, targetPosition);
    //     }
    // }


    // [Command]
    // public void CmdUseAbilityTarget(Unit targetUnit)
    // {
    //     //   abilityWeapon.CmdShoot(targetUnit);
    // }

    [Command]
    public void CmdUseAbilityTarget(Vector3 targetPosition)
    {

        //if target out of range
        if (Vector3.Distance(targetPosition, gameObject.transform.position) > abilityWeapon.getAttackRange())
        {
            TurnOnCeaseFire();
            ServerStopAll();

            base.unitCommandManager.RpcMoveOrder(base.unit, targetPosition);
        }
        Rpc_CR_UseAbility(targetPosition);
    }


    [Command]
    public void CmdStopAll()
    {
        ServerStopAll();
    }
    //a more fitting name would be stop everything
    [Server]
    public void ServerStopAll()
    {
        gameObject.GetComponent<UnitMovement>().ServerStopMoving();

        StopCoroutine(CR_goToTarget);
        CR_goToTarget_isRunning = false;

        StopCoroutine(CR_startShooting);
        CR_startShooting_isRunning = false;

        this.targetedUnit = null;
    }
    [Server]
    public void Die()
    {
        /*
                attackingUnits.Remove(this);
        attack order -> target unit -> unit ->attacking unit -> remove this
        */
        if (targetedUnit)
        {
            targetedUnit.RemoveAttackingUnit(myUnit);
            ServerStopAll();
        }
    }





    // // [Client]
    // // public void ClientStopAttack()
    // // {
    // //     StopCoroutine(CR_goToTarget);
    // //     CR_goToTarget_isRunning = false;
    // //     // gameObject.GetComponent<UnitMovement>().CmdStopMoving();

    // // }

    /*TODO: 
    void AttackMove(){

    }*/

    #endregion

}
