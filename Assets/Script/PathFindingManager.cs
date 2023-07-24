using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathFindingManager : MonoBehaviour
{
    [SerializeField] AstarPath pathFinder;

    // Start is called before the first frame update
    void Start()
    {
        pathFinder.Scan();
    }

    void Scan()
    {
        pathFinder.Scan();
    }

}
