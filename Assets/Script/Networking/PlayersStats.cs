using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayersStats : NetworkBehaviour
{
    [SyncVar] public int team;
    // 0 is when the player is destroyed,
    // 1 is for when the player is alive, 
    // 2 is for the player is in spectator, not inplemented yet  
    [SyncVar, HideInInspector] private int alive;
    [SyncVar, HideInInspector] public int numberOfOwnedUnit = -1;
    [SerializeField] private GameObject gameOverScreen;
    void Start()
    {
        alive = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfOwnedUnit == 0){
            gameOverScreen.SetActive(true);
            alive = 0;
        }
        
    }
}
