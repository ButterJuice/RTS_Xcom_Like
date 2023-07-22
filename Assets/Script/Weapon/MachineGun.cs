using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MachineGun : Weapon
{
    override public float getAttackRange()
    {        
        return attackRange.radius;
    }
    
    #region client
[Command]
    override public void Shoot(Unit targetUnit)
    {        targetUnit.GetUnitStats().loseHealth(10);
    }

    #endregion
}
