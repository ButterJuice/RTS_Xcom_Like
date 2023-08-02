using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class Trowable : NetworkBehaviour
{
    // [SerializeField] public Rigidbody rigidBody;
    [SerializeField] public BallisticMotion ballisticMotion;
    public Vector3 trowForce;

    [SerializeField, Tooltip("This parameter will change the mass of the rigidbody of its game object")] public float mass;
    [SerializeField, Tooltip("This parameter will change the drag of the rigidbody of its game object")] public float drag;

    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().mass = mass;
        gameObject.GetComponent<Rigidbody>().drag = drag;


        //ballisticMotion = GetComponent<BallisticMotion>();
        ballisticMotion.Initialize(this.gameObject.transform.position, -Physics.gravity.y);
        ballisticMotion.AddImpulse(trowForce);




    }
}
