using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class UnitAction : NetworkBehaviour
{
    protected UnitCommandManager unitCommandManager;         
    protected Unit unit;        
    protected void Start()
    {
        unitCommandManager = GameObject.FindWithTag("UnitSelectionManager").GetComponent<UnitCommandManager>();                
        unit = gameObject.GetComponent<Unit>();
    }
    UnitCommandManager getUnitCommandManager()
    {
        return this.unitCommandManager;
    }
}
