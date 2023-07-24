using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [Header("Script")]
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnitAttackOrder unitAttackOrder = null;    
    [SerializeField] private UnitStats unitStats = null;



    public UnitMovement GetUnitMovement(){
        return unitMovement;
    }
    public UnitAttackOrder GetUnitAttackOrder(){
        return unitAttackOrder;
    }
    public UnitStats GetUnitStats(){
        return unitStats;
    }
    #region Server

    #endregion

    #region Client
    [Client]
    public void Select()
    {
        if (!isOwned) return;
        if(onSelected != null) onSelected.Invoke(); 
        else return;
        // equivalent a onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) return;
        if(onDeselected != null) onDeselected.Invoke(); 
        else return;
    }
    #endregion

}
