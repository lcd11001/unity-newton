using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    public Vector3 velocityVector; // average velocity of this FixedUpdate
    public Vector3 netForceVector; // sum of all forces acting on this object
    public List<Vector3> forceVectorList = new List<Vector3>();

    private void FixedUpdate()
    {
        AddForces();

        if (this.netForceVector == Vector3.zero)
        {

            Vector3 deltaS = velocityVector * Time.deltaTime;
            this.transform.position += deltaS;
        }
        else
        {
            Debug.Log("AddForces " + this.netForceVector.ToString() + " " + this.netForceVector.magnitude.ToString());
        }
    }

    void AddForces()
    {
        this.netForceVector = Vector3.zero;
        foreach (Vector3 forceVector in forceVectorList)
        {
            this.netForceVector += forceVector;
        }
    }
}
