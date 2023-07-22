using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class UnitAction : NetworkBehaviour
{
    protected UnitCommandManager unitCommandManager;
    protected void Start()
    {
        unitCommandManager = GameObject.FindWithTag("UnitSelectionManager").GetComponent<UnitCommandManager>();
    }
    UnitCommandManager getUnitCommandManager()
    {
        return this.unitCommandManager;
    }
}
