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
        drawLine(position);

    }

[Client]
    public void drawLine(Vector3 position)
    {
        Vector3 p1 = base.weaponMuzzle.transform.position;
        lr.positionCount = 2;
        lr.SetPosition(0,p1);
        lr.SetPosition(1,position);
    }


    #endregion
}
