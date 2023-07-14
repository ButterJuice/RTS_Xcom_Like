using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager    
{
[SerializeField] private GameObject unitSpawnerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnInstance, conn);
    }


}
