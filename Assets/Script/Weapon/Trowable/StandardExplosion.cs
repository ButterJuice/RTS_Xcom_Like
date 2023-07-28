using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StandardExplosion : NetworkBehaviour
{
    private Collider explosionCollider;
    private float radius = 30f;
    private float takeDamage = 50f;

    [Server]
    void Start()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in collidersHit)
        {
            if (hit.gameObject.TryGetComponent<Unit>(out Unit unit))
            {
                unit.GetUnitStats().takeDamage(50);
            }
        }
    }

    private void Update()
    {
        Destroy(gameObject, 0.1f);
    }

}
