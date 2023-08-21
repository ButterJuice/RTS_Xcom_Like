using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

abstract public class CommandLinkerUI : NetworkBehaviour
{

    // [SerializeField] public GameObject unitCommandManagerGameObject;
    [HideInInspector] public UnitCommandManager unitCommandManager;
    [SerializeField] public Sprite sprite;
    [SyncVar] private Transform myTransform;
    public void Start()
    {
        if (isServer)
        {
            myTransform = gameObject.transform;
        }

    }

    // [Client]
    abstract public void useCommand();
}
