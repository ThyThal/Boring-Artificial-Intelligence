using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentBehavior : FlockingBehavior
{
    public override Vector3 GetDirection(List<Transform> context)
    {
        Vector3 direction = Vector3.zero;

        if (context.Count >= 0)
        {
            // Get direction to nearby allies.
            foreach (Transform item in context)
            {
                FlockingEntity flockEnt = item.GetComponent<FlockingEntity>();
                Vector3 itemDirection = flockEnt.Direction; // FAIL
                direction += itemDirection; // FAIL
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