using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInRadiusBehavior : FlockingBehavior
{
    private float _radius = 10;
    public override Vector3 GetDirection(List<Transform> context)
    {
        Vector3 direction = Vector3.zero;
        Vector3 center = Vector3.zero;

        Vector3 centerOffset = center - this.transform.position;

        float df = centerOffset.magnitude / _radius;

        if (df > .9f)
        {
            direction = centerOffset * df * df;
        }

        return direction;
    }
}