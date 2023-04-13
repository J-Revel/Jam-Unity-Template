using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public EntityPhysicsConfig physicsConfig;
    public Vector3 inputDirection;
    private new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = physicsConfig.mass;
    }
    
    void FixedUpdate()
    {
        rigidbody.AddForce(inputDirection * physicsConfig.acceleration, ForceMode.Acceleration);
        rigidbody.velocity = rigidbody.velocity * Mathf.Pow(physicsConfig.inertia, Time.fixedDeltaTime);
        if(rigidbody.velocity.sqrMagnitude > physicsConfig.maxSpeed * physicsConfig.maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity * physicsConfig.maxSpeed / rigidbody.velocity.magnitude;
        }
    }
}
