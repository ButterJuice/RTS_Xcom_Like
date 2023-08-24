using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

abstract public class CommandLinkerUI : NetworkBehaviour
{

    // [SerializeField] public GameObject unitCommandManagerGameObject;
    [HideInInspector] public UnitCommandManager unitCommandManager;
    [SerializeField] public Sprite sprite;
    public void Start()
    {
        Debug.Log("command linker parent = ",this.transform.parent.gameObject);
    }

    // [Client]
    abstract public void useCommand();
}
