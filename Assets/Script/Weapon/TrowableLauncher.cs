using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TrowableLauncher : Weapon
{

    [SerializeField, Tooltip("The trowable that will be launched")] private Trowable trowable;
    private bool readyToShoot = true;
    private IEnumerator CR_shootCooldown;
    public Vector3 target;
    [SerializeField, Tooltip("The speed at wich the projectile will be launched")] private float projSpeed;
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    private GameObject muzzle;//muzzle refer to the gameObject of the shooting point

    private void Start()
    {
        muzzle = gameObject;//if I have to change the muzzle position I will do it here
        CR_shootCooldown = ShootCooldown();
        StartCoroutine(CR_shootCooldown);
    }

    [Client]
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            shoot();

        }
    }

    [Command]
    private void shoot()
    {
        /*
        To hit our target we need to do some balistic, so here is somewhere I saw some equation
        https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
        the git hub that goes with it:
        https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_ballistic_trajectory.cs
        */

        int numberSolution = fts.solve_ballistic_arc(transform.position, projSpeed, target, -Physics.gravity.y, out Vector3 low, out Vector3 high);

        Vector3 trowForce = Vector3.zero;
        //TODO:

        //To be removed
        if (numberSolution == 0)
        {
            Debug.Log("Pas de trajectoire valid");
            return;
        }
        //  else if (trajectoire valide)
        //  {
        trowForce = high;
        trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, high);
        //  }
        // else 
        // {
        //    trowForce = low;
        //trajectoryPredictor.PredictTrajectory(trowable, muzzle.transform.position, low, projSpeed); // for lower arc
        // }

        if (readyToShoot == true)
        {
            Trowable trowedObject = Instantiate(trowable, muzzle.transform.position, transform.rotation);
            trowedObject.trowForce = trowForce;

            NetworkServer.Spawn(trowedObject.gameObject, connectionToClient);

            readyToShoot = false;
        }

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

    #region Server
    [Command]
    public override void CmdShoot(Unit unit)
    {
        target = unit.transform.position;
        shoot();
    }
    [Command]
    public override void CmdShoot(Vector3 position)
    {
        target = position;
        shoot();
    }
    #endregion
}
