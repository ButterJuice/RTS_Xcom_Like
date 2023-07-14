using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{

    [SerializeField] private Animator unitAnimator = null;
    public NavMeshAgent agent = null;

    [SyncVar] bool isRunning;

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    [Command]
    public void CmdSetRun(bool running)
    {
        isRunning = running;
    }
    #endregion

    #region Client


    [ClientCallback]
    private void Update()
    {
        CmdSetRun(agent.velocity.magnitude > 0f);

        unitAnimator.SetBool("isRunning", isRunning);
    }
    #endregion
}

