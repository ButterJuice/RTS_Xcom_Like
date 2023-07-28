using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class Trowable : NetworkBehaviour
{
    public Rigidbody rigidBody;
    public Vector3 trowForce;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(trowForce,ForceMode.Force);
    }
}
