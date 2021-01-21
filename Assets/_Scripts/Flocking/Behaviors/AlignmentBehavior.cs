using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentBehavior : FlockingBehavior
{
    public override Vector3 GetDirection(List<Transform> context)
    {
        Vector3 direction = Vector3.zero;

        if (context.Count > 0)
        {
            // Get direction to nearby allies.
            foreach (Transform item in context)
            {
                Vector3 itemDirection = item.GetComponent<FlockingEntity>().Direction;
                direction += itemDirection;
            }

            // Get an average position of the context.
            direction /= context.Count;
        }

        else
        {
            direction = this.GetComponent<FlockingEntity>().Direction;
        }

        return direction;
    }
}