using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TrowableLauncher : Weapon
{

    [SerializeField, Tooltip("The trowable that will be launched")] private Trowable trowable;
    private bool readyToShoot = true;
    private IEnumerator CR_shootCooldown;
    public Vector3 target;
    [SerializeField, Tooltip("The speed at wich the projectile will be launched")] private float projSpeed;
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    private GameObject muzzle;//muzzle refer to the gameObject of the shooting point
    [SerializeField, Tooltip("The layers that can be hit by the projectile")] LayerMask hitableLayer;

    private void Start()
    {
        muzzle = base.weaponMuzzle;
        CR_shootCooldown = ShootCooldown();
        StartCoroutine(CR_shootCooldown);
        if(isOwned){
            SetTrajectoryVisible(true);
        }

    }


    [Client]
    void Update()
    {
        // if (Input.GetKey(KeyCode.G))
        // {
        //     CmdShoot(target);

        // }
    }


    [Command]
    public override void CmdShoot(Unit unit)
    {
        Shoot(unit.transform.position);
    }
    [Command]
    public override void CmdShoot(Vector3 position)
    {
        Shoot(position);
    }
    #region Server
    [Server]
    private void Shoot(Vector3 position)
    {
        target = position;
        /*
        To hit our target we need to do some balistic, so here is somewhere I saw some equation
        https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
        the git hub that goes with it:
        https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_ballistic_trajectory.cs
        */

        int numberSolution = fts.solve_ballistic_arc(transform.position, projSpeed, target, -Physics.gravity.y, out Vector3 low, out Vector3 high);

        Vector3 trowForce = Vector3.zero;

        setTrajectoryLine(target);

        float distanceFromTarget = 0;
        Ray rayToTarget = new Ray(muzzle.transform.position, target - muzzle.transform.position);
        if (Physics.Raycast(rayToTarget, out RaycastHit hit, 50000.0f, hitableLayer))
        {
            distanceFromTarget = Vector3.Distance(hit.point, target);
        }

        if (numberSolution == 0)
        {
            Debug.Log("Pas de trajectoire valid");
            return;
        }
        else if (distanceFromTarget > 1) //if (trajectoire valide)
        {
            trowForce = high;
            // trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, high);
        }
        else
        {
            trowForce = low;
            //trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, low); // for lower arc
        }



        if (readyToShoot == true)
        {
            Trowable trowedObject = Instantiate(trowable, muzzle.transform.position, transform.rotation);
            trowedObject.trowForce = trowForce;

            NetworkServer.Spawn(trowedObject.gameObject, connectionToClient);

            readyToShoot = false;
        }

    }

    /*
    setTrajectoryLine won't drawn anything if the line renderer is not set to visible
    To do that you need to use SetTrajectoryVisible;
    */
    [Client]
    public void setTrajectoryLine(Vector3 destination)
    {
        int numberSolution = fts.solve_ballistic_arc(transform.position, projSpeed, destination, -Physics.gravity.y, out Vector3 low, out Vector3 high);


        float distanceFromTarget = 0;
        Ray rayToTarget = new Ray(muzzle.transform.position, destination - muzzle.transform.position);
        if (Physics.Raycast(rayToTarget, out RaycastHit hit, 50000.0f, hitableLayer))
        {
            distanceFromTarget = Vector3.Distance(hit.point, destination);
        }

        if (numberSolution == 0)
        {
            SetTrajectoryVisible(false);
        }
        else if (distanceFromTarget > 1)
        {
            trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, high);
        }
        else
        {
            trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, low); // for lower arc
        }
    }

    [Client]

    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryPredictor.SetTrajectoryVisible(visible);
    }


    IEnumerator ShootCooldown()
    {
        while (true)
        {
            if (readyToShoot == false)
            {
                yield return new WaitForSeconds(0.5f);
                readyToShoot = true;
            }
            yield return null;
        }
    }

    #endregion
}
