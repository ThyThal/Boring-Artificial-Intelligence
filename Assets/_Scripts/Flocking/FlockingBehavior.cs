using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockingBehavior : MonoBehaviour
{
    public abstract Vector3 GetDirection(List<Transform> context);
}
