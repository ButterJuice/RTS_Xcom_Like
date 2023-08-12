using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StandardExplosion : NetworkBehaviour
{
    [SerializeField] private Collider explosionCollider;
    private float radius; // probleme de logique, on a aucune garantie que explosion collider est une sphere   -> faire une nouvelle classe abstraite
    [SerializeField] private float takeDamage = 50f;

    [Server]
    void Start()
    {
        SphereCollider explosionColliderSphere = (SphereCollider) explosionCollider; //ce cast n'est pas safe
        radius = explosionColliderSphere.radius;

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
