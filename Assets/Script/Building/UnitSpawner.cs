using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour//, IPointerClickHandler
{
    [SerializeField] private GameObject[] unitPrefabs = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    public PlayersStats myPlayerStats;
    [SerializeField] public UnitSelection myUnitSelection;
    [SerializeField, Tooltip("0 is me , 1 is others")] Material[] teamColors;//this is currently a duplica of what exist in Unit, it should be removed in the future


    #region Server


    [Server]
    private void SpawnUnit(GameObject unitPrefab, PlayersStats clientPlayerStats)
    {
        GameObject unitSpawn = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitSpawn, connectionToClient);

        // if (isServer) Debug.Log("l'unité a été donné a " + GetComponent<NetworkIdentity>().connectionToClient);


        Unit unitSpawnUnitCompenent = unitSpawn.GetComponent<Unit>();
        unitSpawnUnitCompenent.myPlayerStats = clientPlayerStats;

        myUnitSelection = clientPlayerStats.myUnitSelection;
        unitSpawnUnitCompenent.myUnitSelection = myUnitSelection;

        clientPlayerStats.numberOfOwnedUnit += 1;

        // if(TryGetComponent<Unit>(out Unit unit)){
        //     unit.GetUnitStats().ServerStart();
        // }
    }

    [Command]
    private void CmdSpawnUnit(GameObject unitPrefab)
    {
        SpawnUnit(unitPrefab, myPlayerStats);
    }
    [Command]
    private void CmdSpawnUnits(PlayersStats clientPlayerStats)
    {
        float gap = 0f;
        foreach (GameObject unit in unitPrefabs)
        {
            SpawnUnit(unit, clientPlayerStats);
            unitSpawnPoint.position += new Vector3(2.0f, 0f, 0f);
            gap += 2f;
        }
        unitSpawnPoint.position -= new Vector3(-gap, 0f, 0f);
    }

    #endregion

    #region Client

    private void Start()
    {
        if (!isOwned)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = teamColors[1];
            return;
        }
        else
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                PlayersStats playersStats = player.GetComponent<PlayersStats>();
                if (playersStats.isLocalPlayer)
                {
                    myPlayerStats = playersStats;
                }
            }
            gameObject.GetComponentInChildren<MeshRenderer>().material = teamColors[0];
            Debug.Log(myPlayerStats);
            CmdSpawnUnits(myPlayerStats);

            if (isServer)
            {
                myPlayerStats.alive = 1;
            }
        }
    }





    /*
    //right now this function is used for testing
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            if (!isOwned)
            {
                print("tu n'a pas l'autorité");
                return;
            }

            CmdSpawnUnit();
        }

    */

    #endregion
}