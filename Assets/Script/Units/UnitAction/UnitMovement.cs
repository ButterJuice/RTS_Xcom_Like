using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Pathfinding;

public class UnitMovement : UnitAction
{

    //[SerializeField] private Animator unitAnimator = null;
    public AIPath agent = null;

    [SyncVar] bool isRunning;

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        
        agent.isStopped = false;
        agent.destination = hit.position;
    }
    [Command]
    public void CmdStopMoving()
    {
        agent.isStopped = true;
    }

    [Command]
    public void CmdSetRun(bool running)
    {
        isRunning = running;
    }
    #endregion

    #region Client


    [Client]
    private void Update()
    {
        if(!isOwned) return;

        CmdSetRun(agent.velocity.magnitude > 0f);

      //  unitAnimator.SetBool("isRunning", isRunning);
    }
    #endregion
}

