using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

abstract public class CommandLinkerUI : NetworkBehaviour
{

    [HideInInspector] public UnitCommandManager unitCommandManager;
    [SerializeField] public Sprite sprite;
    public void Start()
    {
        unitCommandManager = GameObject.FindWithTag("UnitSelectionManager").GetComponent<UnitCommandManager>();
        
    }

// [Client]
    abstract public void useCommand();
}
