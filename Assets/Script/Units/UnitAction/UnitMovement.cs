using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Pathfinding;
using Unity.VisualScripting;

public class UnitMovement : UnitAction
{

    private Animator unitAnimator = null;
    public AIPath agent = null;

    [SyncVar] bool isWalking = false;

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }
    [Server]
    public void ServerMove(Vector3 position)
    {

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.isStopped = false;
        agent.destination = hit.position;
    }
    [Command]
    private void CmdStopMoving()
    {
        ServerStopMoving();
    }
    [Server]
    public void ServerStopMoving()
    {
        agent.isStopped = true;
    }
    [Client]
    public void ClientStopMoving()
    {
        CmdStopMoving();
    }

    [Server]
    public void ServerSetRun(bool running)
    {
        isWalking = running;
    }
    #endregion


    new private void Start()
    {
        base.Start();
        agent.isStopped = true;
        unitAnimator = gameObject.GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        // if(!isOwned) return;

        if (isServer)
        {
            ServerSetRun(agent.velocity.magnitude > 0.01f);

            if (Vector3.Distance(agent.destination, gameObject.transform.position) < 1)
            {

                agent.isStopped = true;
            }
        }
        //unitAnimator.SetBool("isWalking", isWalking);
        if (isClient)
        {
            unitAnimator.SetBool("isWalking", isWalking);
        }

    }
}

