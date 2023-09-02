using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;
using UnityEngine.UI;

/*
The naming have become wrong, it should just be player and this class contain all unique player related thing
*/
public class PlayersStats : NetworkBehaviour
{
    [SyncVar] public int team;

    // -1 is when the player waiting/not ready,
    // 0 is when the player is destroyed,
    // 1 is for when the player is alive, 
    // 2 is for the player is in spectator, not implemented yet  
    [SyncVar, HideInInspector] public int alive;
    [SyncVar, HideInInspector] public int numberOfOwnedUnit;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject unitSelectionGameObject;
    [HideInInspector] public UnitSelection myUnitSelection;
    void Start()
    {
        numberOfOwnedUnit = 0;
        alive = -1;
        myUnitSelection = unitSelectionGameObject.GetComponent<UnitSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive == -1) return;

        if (numberOfOwnedUnit == 0 & alive == 1)
        {
            // if (isClient){
            if (isServer)
            {

                alive = 0;
            }
            if (isLocalPlayer)
            {
                // RpcShowDefeat();
                gameOverScreen.SetActive(true);
            }
            // }
        }
    }
    [ClientRpc]
    public void RpcShowDefeat()
    {
        gameOverScreen.SetActive(true);
    }

    [Server]
    public void UnitDie(Unit deadUnit)
    {
        numberOfOwnedUnit -= 1;

        Debug.Log(" unit count = " + numberOfOwnedUnit);
        // GetComponent<Unit>().myPlayer.numberOfOwnedUnit -= 1;
    }
}
