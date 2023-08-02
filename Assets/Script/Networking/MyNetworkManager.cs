using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    private int playerNumber = 0;
    [SerializeField] private GameObject playerList; //to keep scene clean

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        playerNumber += 1;
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnInstance, conn);

        GameObject playerInstance = Instantiate(base.playerPrefab, playerList.transform);
        NetworkServer.Spawn(unitSpawnInstance, conn);

        base.playerPrefab.GetComponent<PlayersStats>().team = playerNumber;
    }


}
