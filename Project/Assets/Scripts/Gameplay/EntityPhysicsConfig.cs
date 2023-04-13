using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Physics/Physics Config")]
public class EntityPhysicsConfig : ScriptableObject
{
    public float acceleration;
    public float inertia;
    public float maxSpeed;
    public float mass;
    public float gravity;
}
