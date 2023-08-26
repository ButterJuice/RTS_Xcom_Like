using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

abstract public class CommandLinkerUI : MonoBehaviour
{

    // [SerializeField] public GameObject unitCommandManagerGameObject;
    [HideInInspector] public UnitCommandManager unitCommandManager;
    [SerializeField] public Sprite sprite;
    public void Start()
    {

    }

    abstract public void useCommand();
}
