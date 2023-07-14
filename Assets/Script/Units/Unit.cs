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

    public UnitMovement GetUnitMovement(){
        return unitMovement;
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
        //onSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!isOwned) return;
        if(onDeselected != null) onDeselected.Invoke(); 
        else return;
        //onSelected?.Invoke();
    }
    #endregion

}
