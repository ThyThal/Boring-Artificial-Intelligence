using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceBehavior : FlockingBehavior
{
    public float _personalSpaceRadius = 3;

    public override Vector3 GetDirection(List<Transform> context)
    {
        Vector3 direction = Vector3.zero;

        foreach (Transform item in context)
        {
            Vector3 offset = this.transform.position - item.transform.position;

            // If item is closer than personal space, space myself.
            if (offset.magnitude < _personalSpaceRadius)
            {

                float scale = offset.magnitude / Mathf.Sqrt(_personalSpaceRadius);

                Vector3 forceVector = offset.normalized / scale;

                direction += forceVector;
            }
        }

        return direction;
    }
}