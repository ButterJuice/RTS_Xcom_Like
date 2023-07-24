using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MachineGun : Weapon
{
    [SerializeField] private LineRenderer lr = null;//Could be a get compenent on the WeaponMuzzle Gameobject

    #region Server
    [Command]
    override public void CmdShoot(Unit targetUnit)
    {
        targetUnit.GetUnitStats().takeDamage(10);
        RpcShootAnimation(targetUnit.transform.position);
    }

    #endregion



    #region Client
    [ClientRpc]
    private void RpcShootAnimation(Vector3 position)
    {   /*
    TODO: call the animation
    */
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
        lr.enabled = true;
        Vector3 p1 = base.weaponMuzzle.transform.position;
        lr.positionCount = 2;
        lr.SetPosition(0, p1);
        lr.SetPosition(1, position);
       yield return new WaitForSeconds(0.5f);
        lr.enabled = false;
    }



    #endregion
}
