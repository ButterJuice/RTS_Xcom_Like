using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;
using UnityEngine.InputSystem;

public class CmdLinkerLaunchGrenade : CommandLinkerUI
{

    private bool targeting;//true while the player chose a target
    private Camera mainCamera;
    [SerializeField, Tooltip("Layer of potential target")] private LayerMask groundLayer;
    [Tooltip("The abilityWeapon of the unit")] private TrowableLauncher trowableLauncher;
    protected new virtual void Start()
    {
        base.Start();
        mainCamera = Camera.main;
    }

    public override void useCommand()
    {
        // Debug.Log("Le bouton de l'UI marche");
        // base.unitCommandManager.AbilityOrder(unitCommandManager.transform.position);


        Unit abilityUser = base.unitCommandManager.returnFirstUnit();

        if (abilityUser == null)
        {
            return;
        }

        trowableLauncher = abilityUser.GetComponentInChildren<TrowableLauncher>();
        StartCoroutine(chooseTargetCoroutine(abilityUser));
    }

    IEnumerator chooseTargetCoroutine(Unit abilityUser)
    {
        targeting = true;
        while (targeting)
        {
            Vector3 targetLocation = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(targetLocation);


            if (Physics.Raycast(ray, out RaycastHit hit, 50000.0f, groundLayer))
            {
                trowableLauncher.SetTrajectoryVisible(true);
                trowableLauncher.setTrajectoryLine(hit.point);
                // Debug.DrawRay(mainCamera.transform.position, hit.point - mainCamera.transform.position,Color.red,5f);
                if (Input.GetKey(KeyCode.G))
                {
                    // Debug.DrawRay(mainCamera.transform.position, hit.point -  mainCamera.transform.position,Color.yellow,5f);

                    //  abilityUser.PrintMyself();

                    base.unitCommandManager.AbilityOrder(abilityUser, hit.point);
                    trowableLauncher.SetTrajectoryVisible(false);
                    targeting = false;
                }
            }


            yield return null;
        }
        yield return null;
    }



}
