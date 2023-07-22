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
public void loseHealth(float healthLost) {
    health -= healthLost;
    if(health < 0){
        Die();
    }
}

[ClientRpc]
void Die(){
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
