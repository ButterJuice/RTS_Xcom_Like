using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlotUI : NetworkBehaviour
{
    [SerializeField] public GameObject commandPannelGameObject;

    [HideInInspector] public CommandPannelUI commandPannel;

    [SerializeField] private Button myButton;
    [SerializeField, Tooltip("GameObject that will execute the command corresponding to the ability")] public GameObject commandLinkerObject;
    private CommandLinkerUI commandLinker;

    public UnityEngine.UI.Image imageComponent;
    // [SyncVar] Transform myTransform;



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
        if(isClient){
            this.commandLinkerObject = null;
        }
        // commandPannelGameObject = GameObject.Find("CommandPanel");
        // this.gameObject.transform.SetParent(commandPannelGameObject.transform);

        // commandPannelGameObject = GetComponentInParent<InstantiateCommandSlotUI>().commandPannelGameObject;

        // commandPannel = commandPannelGameObject.GetComponent<CommandPannelUI>();



        // if (isServer)
        // {
        //     // myTransform = transform;
        //     // myTransform.SetParent(gameObject.transform.parent);

        //     commandPannel = GetComponentInParent<CommandPannelUI>();

        //     commandLinkerObject = Instantiate(commandLinkerObject, transform);
        //     NetworkServer.Spawn(commandLinkerObject, connectionToClient);
        //     // RpcSetParent(commandLinkerObject);

        //     RpcFinishStart(commandLinkerObject);

        //     // commandLinker = commandLinkerObject.GetComponent<CommandLinkerUI>();
        //     // commandLinker.unitCommandManager = commandPannel.unitCommandManager;

        //     // commandPannelGameObject = transform.parent.parent.gameObject;
        //     // RpcClientStart(commandLinkerObject); //commandPannelGameObject
        // }

    }

    [Server]
    public void serverFinishStart()
    {

            commandPannel = GetComponentInParent<CommandPannelUI>();

            commandLinkerObject = Instantiate(commandLinkerObject, transform);
            NetworkServer.Spawn(commandLinkerObject, connectionToClient);

        commandLinkerObject.name= "bbbbbbbbbbbbbb";
        Debug.Log("commandLinker = ",commandLinkerObject);

            RpcFinishStart(commandLinkerObject);


    }
    [ClientRpc]
    public void RpcFinishStart(GameObject newCommandLinkerObject)
    {
        this.commandLinkerObject = newCommandLinkerObject;
        
        commandLinkerObject.name= "aaaaaaaa";
        Debug.Log("commandLinker = ",commandLinkerObject);

        commandLinkerObject.transform.SetParent(this.gameObject.transform);
        

        commandPannel = GetComponentInParent<CommandPannelUI>();

        commandLinker = commandLinkerObject.GetComponent<CommandLinkerUI>();
        commandLinker.unitCommandManager = commandPannel.unitCommandManager;

        imageComponent.sprite = commandLinker.sprite;
        myButton.onClick.AddListener(commandLinker.useCommand);


    }
    void Update(){
        
    }

    // [ClientRpc]
    // void RpcSetParent(GameObject child) //GameObject commandPannelGameObject
    // {
    //     child.transform.SetParent(this.gameObject.transform);
    // }


}
