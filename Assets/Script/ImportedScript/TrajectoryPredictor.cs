using System;
using System.Collections;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
/*
This script come from this video:
https://www.youtube.com/watch?v=rgJGkYM2_6E
and the github that goes with it:
https://github.com/ForlornU/Trajectory-Predictor/blob/main/Assets/Scripts/ProjectileProperties.cs

note that I modified a few part in the script to go with my needs

*/
public class TrajectoryPredictor : NetworkBehaviour
{
    #region Members
    LineRenderer trajectoryLine;
    [SerializeField, Tooltip("The marker will show where the projectile will hit")]
    Transform hitMarker;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
    int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
    float increment = 0.5f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float rayOverlap = 1.1f;
    [SerializeField, Tooltip("The layers that can hit")]
    LayerMask hitableLayer;
    #endregion

    private void Start()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(true);
    }



/*
If I want a prediction based on a direction and a velocity then I can use this:
    public void PredictTrajectory(Trowable projectile, Vector3 initialPosition,Vector3 direction, float initialSpeed)
    {
        Vector3 velocity = initialSpeed / projectile.mass * direction;
        .
        .
        .
    }
*/
    public void PredictTrajectory(Trowable projectile, Vector3 initialPosition, Vector3 force)
    {
        Vector3 velocity = force;
        Vector3 position = initialPosition;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(maxPoints, (0, position));

        for (int i = 1; i < maxPoints; i++)
        {
            // Estimate velocity and update next predicted position
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap, hitableLayer))
            {
                UpdateLineRender(i, (i - 1, hit.point));

                MoveHitMarker(hit);
                break;
            }

            //If nothing is hit, continue rendering the arc without a visual marker
            hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }
    /// <summary>
    /// Allows us to set line count and an individual position at the same time
    /// </summary>
    /// <param name="count">Number of points in our line</param>
    /// <param name="pointPos">The position of an induvidual point</param>
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        trajectoryLine.positionCount = count;
        if (pointPos.pos.y != Mathf.Infinity)
            trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        // velocity += (Physics.gravity) * increment;//inexacte je sais pas pourquoi
        velocity += (Physics.gravity+0.5f*Vector3.up) * increment;//correction moche a cause d'imprecision
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    private void MoveHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);

        // Offset marker from surface
        float offset = 0.025f;
        hitMarker.position = hit.point + hit.normal * offset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

[Client]
    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }

}