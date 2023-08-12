using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private int playerNumber = 0;
    [SerializeField] private GameObject playerList; //to keep scene clean

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        playerNumber += 1;
        base.OnServerAddPlayer(conn);
        base.playerPrefab.GetComponent<PlayersStats>().team = playerNumber;

        GameObject playerInstance = Instantiate(base.playerPrefab, playerList.transform);
        NetworkServer.Spawn(playerInstance, conn);

        
    }

}
