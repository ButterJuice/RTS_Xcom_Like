using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;


    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitSpawn = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitSpawn, connectionToClient);
    }
    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!isOwned) { 
            print("tu n'a pas l'autorité");
            return; }

        CmdSpawnUnit();
    }
    #endregion
}

 