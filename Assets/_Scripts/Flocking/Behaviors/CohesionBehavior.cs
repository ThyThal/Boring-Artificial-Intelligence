using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionBehavior : FlockingBehavior
{
    public override Vector3 GetDirection(List<Transform> context)
    {
        Vector3 direction = Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        FlockingEntity entity = this.GetComponent<FlockingEntity>();

        if (context.Count > 0)
        {
            foreach (Transform item in context)
            {
                centerOfMass += item.position;
            }

            centerOfMass /= context.Count;

            direction = centerOfMass - this.transform.position;
        }

        return direction;
    }
}