using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using UnityEngine;


/*
This script and its game object are made because we cannot have 2 network identity on the same game object but the Command slot need a network identity and the canvas to
*/
public class InstantiateCommandSlotUI : NetworkBehaviour
{
    [SerializeField] public GameObject commandSlotGameObject;
    [SerializeField] public GameObject commandPannelGameObject;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            commandSlotGameObject = Instantiate(commandSlotGameObject, this.transform);
            NetworkServer.Spawn(commandSlotGameObject, connectionToClient);
            RpcSetParent(commandSlotGameObject);
            CommandSlotUI commandSlotScript = commandSlotGameObject.GetComponent<CommandSlotUI>();

            commandSlotScript.serverFinishStart();
            // commandSlotScript.RpcFinishStart();

            // commandSlotGameObject.GetComponent<CommandSlotUI>().RpcFinishStart();
        }
        // if (isClient)
        // {
        //     commandSlotGameObject = Instantiate(commandSlotGameObject, this.transform);
        //     clientInstantiate(commandSlotGameObject)
        // }
    }
    // [Command]
    // public void clientInstantiate(GameObject commandSlotGameObject)
    // {
    //     NetworkServer.Spawn(commandSlotGameObject, connectionToClient);
    // }


    [ClientRpc]
    void RpcSetParent(GameObject child) //GameObject commandPannelGameObject
    {
        child.transform.SetParent(this.gameObject.transform);
    }

}
