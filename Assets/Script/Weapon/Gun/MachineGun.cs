using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MachineGun : Weapon
{
    [SerializeField] private LineRenderer lr = null;//Could be a get compenent on the WeaponMuzzle Gameobject
    

    #region Server
    [Server]
    public override void Shoot(Unit targetUnit)
    {
        targetUnit.GetUnitStats().takeDamage(10);
        Vector3 closestPoint = targetUnit.GetComponent<Collider>().ClosestPoint(base.weaponMuzzle.transform.position);
        RpcShootAnimation(targetUnit.GetComponent<Collider>().ClosestPoint(base.weaponMuzzle.transform.position));
    }


    [Server]
    public override void Shoot(Vector3 position)
    {
        RpcShootAnimation(position);
        Debug.LogWarning("CmdShoot(Position) de MachineGun a été appellée, ce script n'est pas encore entierement implémenté");
    }

    [Command]
    public override void CmdShoot(Vector3 position)
    {
        Shoot(position);
    }

    #endregion



    #region Client
    [ClientRpc]
    private void RpcShootAnimation(Vector3 position)
    {  
        StartCoroutine(drawLineCoroutine(position));

    }
/*
    [Client]
    public void drawLine(Vector3 position)
    {
        Vector3 p1 = base.weaponMuzzle.transform.position;
        lr.positionCount = 2;
        lr.SetPosition(0, p1);
        lr.SetPosition(1, position);
    }
*/

    [Client]
    IEnumerator drawLineCoroutine(Vector3 position)
    {
        Debug.Log(unitAnimator);
        base.unitAnimator.SetBool("isShooting", true);

        lr.enabled = true;
        Vector3 p1 = base.weaponMuzzle.transform.position;
        lr.positionCount = 2;
        lr.SetPosition(0, p1);
        lr.SetPosition(1, position);
       yield return new WaitForSeconds(0.1f);
        lr.enabled = false;
        
        base.unitAnimator.SetBool("isShooting", false);
    }



    #endregion
}
