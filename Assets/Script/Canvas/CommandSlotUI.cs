using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlotUI : MonoBehaviour
{
    [SerializeField] public GameObject commandPannelGameObject;

    [HideInInspector] public CommandPannelUI commandPannel;

    [SerializeField] private Button myButton;
    [SerializeField, Tooltip("GameObject that will execute the command corresponding to the ability")] public GameObject commandLinkerObject;
    public UnityEngine.UI.Image imageComponent;

    private CommandLinkerUI commandLinker;

    [HideInInspector] public UnitCommandManager unitCommandManager;


    // void Start()
    // {
        
    //     commandPannel = commandPannelGameObject.GetComponent<CommandPannelUI>();

    //     commandLinkerObject = Instantiate(commandLinkerObject, transform);
    //     commandLinker = commandLinkerObject.GetComponent<CommandLinkerUI>();

    //     imageComponent.sprite = commandLinker.sprite;
    //     myButton.onClick.AddListener(commandLinker.useCommand);

    //     commandLinker.unitCommandManager = commandPannel.unitCommandManager;
    // }


    void Start()
    {
        commandPannel = commandPannelGameObject.GetComponent<CommandPannelUI>();


            commandLinkerObject = Instantiate(commandLinkerObject, transform);

            commandLinker = commandLinkerObject.GetComponent<CommandLinkerUI>();
            commandLinker.unitCommandManager = commandPannel.unitCommandManager;

        imageComponent.sprite = commandLinker.sprite;
        myButton.onClick.AddListener(commandLinker.useCommand);
        

    }
    
}
