using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitStats : NetworkBehaviour
{
    [SyncVar] public float health;
    [SyncVar] public int team;
    
    #region Server
[Server]
public void takeDamage(float healthLost) {
    health -= healthLost;
    if(health <= 0){
        RpcDie();
    }
}

[ClientRpc]
/*
TODO:
change it to die animation and die should be another procedure on the server (for the case where they are special effect)
*/
void RpcDie(){
    //those 2 line are mostly for testing purpose they will change in the future
    gameObject.transform.GetChild(0).gameObject.SetActive(false);
    gameObject.transform.GetChild(1).gameObject.SetActive(true);
}

    #endregion


    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
    }
}
