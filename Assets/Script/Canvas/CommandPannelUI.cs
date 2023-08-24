using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPannelUI : MonoBehaviour
{
    [SerializeField] public Canvas canvas;
    [SerializeField] private Image image;
    
    [SerializeField] public GameObject unitCommandManagerGameObject;
    [HideInInspector] public UnitCommandManager unitCommandManager;
    // Start is called before the first frame update
    void Start()
    {
        unitCommandManager = unitCommandManagerGameObject.GetComponent<UnitCommandManager>();

    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
