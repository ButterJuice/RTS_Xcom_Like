using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitStats : NetworkBehaviour
{
    [SyncVar] public float health;
    [SyncVar] public int team;
    [SerializeField] private Transform leftEmplacement;
    [SerializeField] private Transform topRightEmplacement;
    [SerializeField, Tooltip("This is the weapon used for auto-attacking")] public Weapon mainWeapon;
    [SerializeField, Tooltip("this is the weapon used for ability")] public Weapon abilityWeapon;
    [SyncVar] public Transform mainWeaponTransform;
    [SyncVar] public Transform abilityWeaponTransform;




    // [SerializeField, Tooltip("This is the default weapon used for auto-attacking")] private Weapon defaultMainWeapon;
    // [SerializeField, Tooltip("this is the default weapon used for ability")] private Weapon defaultAbilityWeapon;
    // [HideInInspector] public Weapon mainWeapon;
    // [HideInInspector] public Weapon abilityWeapon;



    private void Start()
    {
        // //this is nessecary to instentiate this here as it is used in Start of unitAttackOrder
        // mainWeapon = Instantiate(mainWeapon, leftEmplacement);//this also remove the notion of prefab
        // abilityWeapon = Instantiate(abilityWeapon, topRightEmplacement);

        // mainWeapon = Instantiate(mainWeapon, leftEmplacement);//this also remove the notion of prefab
        // abilityWeapon = Instantiate(abilityWeapon, topRightEmplacement);
        if (isServer)
        {
            mainWeapon = Instantiate(mainWeapon, leftEmplacement);//this also remove the notion of prefab
            abilityWeapon = Instantiate(abilityWeapon, topRightEmplacement);

            GetComponent<UnitAttackOrder>().WeaponInnit();

            NetworkServer.Spawn(mainWeapon.gameObject, connectionToClient);
            NetworkServer.Spawn(abilityWeapon.gameObject, connectionToClient);

            CorrectPositon(mainWeapon.gameObject, abilityWeapon.gameObject);
        }

        GetComponent<UnitAttackOrder>().WeaponInnit();

    }
    



    //I am not sure this code is correct and I should somehow sync the transform
    [ClientRpc]
    public void CorrectPositon(GameObject originalMainWeapon, GameObject originalAbilityWeapon)
    {
        originalMainWeapon.transform.position = leftEmplacement.position;
        originalMainWeapon.transform.rotation = leftEmplacement.rotation;
        originalMainWeapon.transform.SetParent(leftEmplacement);

        originalAbilityWeapon.transform.position = topRightEmplacement.position;
        originalAbilityWeapon.transform.rotation = topRightEmplacement.rotation;
        originalAbilityWeapon.transform.SetParent(topRightEmplacement);

        this.mainWeapon = originalMainWeapon.GetComponent<Weapon>();
        this.abilityWeapon = originalAbilityWeapon.GetComponent<Weapon>();
    }

    #region Server

    [Server]
    public void takeDamage(float healthLost)
    {
        health -= healthLost;
        if (health <= 0)
        {
            RpcDie();
        }
    }
    #endregion

    [ClientRpc]
    /*
    TODO:
    change it to die animation and die should be another procedure on the server (for the case where they are special effect)
    */
    void RpcDie()
    {
        //those 2 line are mostly for testing purpose they will change in the future
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }



}
