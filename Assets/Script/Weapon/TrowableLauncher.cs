using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowableLauncher : MonoBehaviour
{

    [SerializeField] private Trowable trowable;
    private bool readyToShoot = true;
    private IEnumerator CR_shootCooldown;
    [SerializeField] public GameObject target; //[SerialisedField] will be removed
    [SerializeField] private float proj_speed;

    private void Start()
    {

        CR_shootCooldown = ShootCooldown();
        StartCoroutine(CR_shootCooldown);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            if (readyToShoot == true)
            {

                /*
                To hit our target we need to do some balistic, so here is somewhere I saw some equation
                https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
                the git hub that goes with it:
                https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_ballistic_trajectory.cs
                */
                int numberSolution = fts.solve_ballistic_arc(transform.position, proj_speed, target.transform.position, 9.81f, out Vector3 low, out Vector3 high);
                Vector3 trowForce = Vector3.zero;
                if (numberSolution == 0)
                {
                    Debug.Log("Pas de trajectoire valid");
                    return;
                }
                else if (low != Vector3.zero)
                {
                    trowForce = low;
                }
                else if (high != Vector3.zero)
                {
                    trowForce = high;
                }
                Trowable trowedObject = Instantiate(trowable, transform.position, transform.rotation);
                trowedObject.trowForce = trowForce;
                readyToShoot = false;
            }
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
}
