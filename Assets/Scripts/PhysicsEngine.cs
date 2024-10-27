using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    public Vector3 velocityVector; // average velocity of this FixedUpdate

    private void FixedUpdate()
    {
        Vector3 deltaS = velocityVector * Time.deltaTime;
        this.transform.position += deltaS;
    }
}
