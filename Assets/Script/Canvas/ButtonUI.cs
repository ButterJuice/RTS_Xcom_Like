using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ButtonUI : NetworkBehaviour
{

    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private Canvas canvas;


public void Start() {
    if(!isLocalPlayer) canvas.gameObject.SetActive(false);
    // Debug.Log(gameObject);
}
    [Client]
    public void PlayerReadyButton()
    {
        if(!isOwned) return;
        PlayerReady(connectionToClient);
    }

    [Command]
    public void PlayerReady(NetworkConnectionToClient conn){

        GameObject unitSpawnInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnInstance, conn);


    }
}
