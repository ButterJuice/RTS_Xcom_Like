using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour//, IPointerClickHandler
{
    [SerializeField] private GameObject[] unitPrefabs = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    public PlayersStats myPlayer;
    [SerializeField, Tooltip("0 is me , 1 is others")] Material[] teamColors;//this is currently a duplica of what exist in Unit, it should be removed in the future


    #region Server

    [Command]
    private void CmdSpawnUnit(GameObject unitPrefab)
    {
        GameObject unitSpawn = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitSpawn, connectionToClient);
        unitPrefab.GetComponent<Unit>().myPlayer = myPlayer;
    }
    [Server]
    private void SpawnUnit(GameObject unitPrefab)
    {
        GameObject unitSpawn = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitSpawn, connectionToClient);
        unitPrefab.GetComponent<Unit>().myPlayer = myPlayer;
    }
    [Command]
    private void CmdSpawnUnits()
    {
        float gap = 0f;
        foreach (GameObject unit in unitPrefabs)
        {
            SpawnUnit(unit);
            unitSpawnPoint.position += new Vector3(2.0f, 0f, 0f);
            gap += 2f;
        }
        unitSpawnPoint.position -= new Vector3(-gap, 0f, 0f);
    }

    #endregion

    #region Client

    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PlayersStats playersStats = player.GetComponent<PlayersStats>();
            if (playersStats.isOwned)
            {
                myPlayer = playersStats;
            }
        }
        if (isOwned)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = teamColors[0];
        }
        else
        {

            gameObject.GetComponentInChildren<MeshRenderer>().material = teamColors[1];
        }

        CmdSpawnUnits();
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