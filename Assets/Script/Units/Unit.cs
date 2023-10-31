using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SyncVar] private List<Unit> attackingUnits = new List<Unit>(); //list of unit that attack this unit
    [SerializeField] private UnityEvent onDeselected = null;
    [Header("Script")]
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnitAttackOrder unitAttackOrder = null;
    [SerializeField] private UnitStats unitStats = null;
    [HideInInspector] public PlayersStats myPlayerStats;
    [HideInInspector] public UnitSelection myUnitSelection;
    [SerializeField, Tooltip("0 is me , 1 is others")] Material[] teamColors;
    [SerializeField, Tooltip("Prefab that will change material depending on team")] GameObject[] teamPrefab;
    private Animator unitAnimator = null;
    [SyncVar] bool isDead = false; //actuellement inutile parce le serveur suprime l'objet

    private void Start()
    {
        unitAnimator = gameObject.GetComponentInChildren<Animator>(); 
        // if(isServer){
        // myPlayer.numberOfOwnedUnit += 1;
        // }

        if (isOwned)
        {
            foreach (GameObject prefab in teamPrefab)
            {
                prefab.GetComponent<MeshRenderer>().material = teamColors[0];
            }
        }
        else
        {
            foreach (GameObject prefab in teamPrefab)
            {
                prefab.GetComponent<MeshRenderer>().material = teamColors[1];
            }
        }
    }
    [Server]
    public void AddAttackingUnit(Unit attackingUnit)
    {
        this.attackingUnits.Add(attackingUnit);
    }
    [Server]
    public void RemoveAttackingUnit(Unit attackingUnit)
    {
        this.attackingUnits.Remove(attackingUnit);
    }
    [Server]
    public void Die()
    {
        isDead = true;
        // attackingUnits.Remove(this);
        
        this.unitAttackOrder.Die();

        foreach (Unit attackingUnit in attackingUnits)
        {


            if (attackingUnit != null)
            {
                if (attackingUnit.gameObject != null)
                    attackingUnit.UnitKilled();
            }
        }
        RpcDie();

        myPlayerStats.UnitDie(this);
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcDie()
    {
        //The drath animation doesn't work because the object is destroyed, I will still leave it here in case one day I want to change the code
        unitAnimator.SetBool("isDead", isDead);
        myPlayerStats.myUnitSelection.Deselect(this);
        //those 2 line are mostly for testing purpose they will change in the future
       // gameObject.transform.GetChild(0).gameObject.SetActive(false);//0 here is the normal color cube
       // gameObject.transform.GetChild(1).gameObject.SetActive(true);//1 here is the red color cube

    }

    public void UnitKilled()//When this unit targets get killed
    {
        this.unitAttackOrder.ServerStopAll();
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    public UnitAttackOrder GetUnitAttackOrder()
    {
        return unitAttackOrder;
    }
    public UnitStats GetUnitStats()
    {
        return unitStats;
    }
    #region Server

    #endregion

    #region Client
    [Client]
    public void Select()
    {
        if (!isOwned) return;
        if (onSelected != null) onSelected.Invoke();
        else return;
        // equivalent a onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) return;
        if (onDeselected != null) onDeselected.Invoke();
        else return;
    }
    #endregion


    public void PrintMyself()
    {
        gameObject.name = "PrintedUnit" + unitStats.team;
        Debug.Log(gameObject);
    }
}
