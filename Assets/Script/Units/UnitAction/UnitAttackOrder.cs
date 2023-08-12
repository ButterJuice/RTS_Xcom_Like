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
        CR_startShooting = StartShooting();
        CR_goToTarget = GoToTarget();

    }



    #region Client
    [Client]
    private void Update()
    {

        if (!isOwned) return;

        if (targetedUnit && !ceaseFire)
        {
            attackTarget();
        }
        else return;
    }

    /*
    This function is the manager of auto attack, it will make the unit follow the target and shoot it if in range
    */
    [Client]
    void attackTarget()
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
    public void TurnOnCeaseFire()
    {
        ceaseFire = true;
    }
    public void TurnOffCeaseFire()
    {
        ceaseFire = false;
    }

    [Client]
    IEnumerator GoToTarget()
    {
        while (true)
        {
            if (targetedUnit && !targetedUnit)
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
            if (targetedUnit && !ceaseFire)//targetedUnit is here to prevent on error that is caused if there is no target as it is a coroutine
            {
                // test();
                mainWeapon.CmdShoot(targetedUnit);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // public void test()
    // {
    //     mainWeapon.CmdShoot(targetedUnit);
    // }

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

            while (true)
            {

                if (Vector3.Distance(targetPosition, gameObject.transform.position) <= abilityWeapon.getAttackRange())
                {
                    abilityWeapon.CmdShoot(targetPosition);
                    break;
                }
                yield return null;
            }
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
            base.unitCommandManager.RpcMoveOrder(closestPointOfTarget);
        }
    }

    [Command]
    public void CmdAttackTarget(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, gameObject.transform.position) > weaponAttackRange)
        {
            base.unitCommandManager.RpcMoveOrder(targetPosition);
        }
    }


    [Command]
    public void CmdUseAbilityTarget(Unit targetUnit)
    {
        //   abilityWeapon.CmdShoot(targetUnit);
    }

    [Command]
    public void CmdUseAbilityTarget(Vector3 targetPosition)
    {
        //if target out of range
        if (Vector3.Distance(targetPosition, gameObject.transform.position) > abilityWeapon.getAttackRange())
        {
            TurnOnCeaseFire();
            StopAttack();

            base.unitCommandManager.RpcMoveOrder(targetPosition);
        }
        Rpc_CR_UseAbility(targetPosition);
    }


    [Command]
    public void CmdStopAttack()
    {
        StopAttack();
    }
    [Server]
    public void StopAttack()
    {
        this.targetedUnit = null;
    }
    /*TODO: 
    void AttackMove(){

    }*/

    #endregion

}
