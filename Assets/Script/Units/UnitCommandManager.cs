using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Globalization;

public class UnitCommandManager : NetworkBehaviour
{

    [Header("Script")]
    [SerializeField] private UnitSelection unitSelection = null;
    private Camera mainCamera;
    [SerializeField] private LayerMask GroundLayer = new LayerMask();
    [SerializeField] private LayerMask InteractableLayer = new LayerMask();

    [Client]
    private void Start()
    {
        //  if(isServer && !isLocalPlayer) gameObject.SetActive(false);
        if (!isLocalPlayer) gameObject.SetActive(false);
        mainCamera = Camera.main;//The main camera need to have the tag "MainCamera"
    }

    [Client]
    private void Update()
    {


        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
        CmdStopOrder();//Could be moved elsewere if we only want to stop action if the requested action is valid 
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        //In order to input command we will use 2 rays that each have a layers, first we try to see if we hit an interactable 
        //(something a unit can interact with, even other units) or if it fail, the ground
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, InteractableLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 1.0f);
            if (hit.collider.TryGetComponent<Unit>(out Unit targetUnit))
            {

                // AbilityOrder(unit.transform.position);
                foreach (Unit unit in unitSelection.selectedUnits)
                {

                    AttackOrder(unit, targetUnit);
                }
            }

        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.green, 1.0f);

            foreach (Unit unit in unitSelection.selectedUnits)
            {
                MoveOrder(unit, hit.point);
            }


        }
        else return;
    }


    /*
    The possible actions are listed below this comment.
    note that the action themself can call other actions 
    for exemple an attack order on the ground will call move unit until there is a unit to target
    */
    [Client]
    public void MoveOrder(Unit movingUnit, Vector3 point)
    {

        if (!movingUnit.isOwned) return;
        movingUnit.GetUnitAttackOrder().cmdStopAttacking();
        movingUnit.GetUnitMovement().CmdMove(point);
    }
    [ClientRpc]
    public void RpcMoveOrder(Unit movingUnit, Vector3 point)
    {
        MoveOrder(movingUnit, point);
    }
    [Client]
    public void AttackOrder(Unit attackingUnit, Unit targetUnit)
    {

        if (attackingUnit == targetUnit) return;

        attackingUnit.GetUnitAttackOrder().CmdAttackTarget(targetUnit);


    }

    [Client]
    public void AbilityOrder(Unit abilityUser, Vector3 targetPosition)
    {
        abilityUser.GetUnitAttackOrder().CmdUseAbilityTarget(targetPosition);

    }

    //this founction is also used every time the user click without holding shift
    [Command]
    public void CmdStopOrder()
    {

        foreach (Unit unit in unitSelection.selectedUnits)
        {

            unit.GetUnitMovement().ServerStopMoving();
            unit.GetUnitAttackOrder().ServerStopAll();
        }
    }

    [Client]
    public Unit returnFirstUnit()
    {
        try
        {
            return unitSelection.selectedUnits.First();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
