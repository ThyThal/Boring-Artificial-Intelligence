using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingEntity : MonoBehaviour
{
    private float _flockRadius = 5f;
    private Vector3 _direction;
    private LayerMask _alliesLayer;
    private Collider _myCollider;
    private FlockingBehavior[] _flockingBehaviors;

    private void Awake()
    {
        _myCollider = this.GetComponent<Collider>();
        _flockingBehaviors = this.GetComponents<FlockingBehavior>();
    }

    public Vector3 Direction
    {
        get
        {
            return _direction;
        }
    }

    public List<Transform> GetNearbyEntities()
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(this.transform.position, _flockRadius, _alliesLayer);

        foreach (Collider collider in contextColliders)
        {
            if (collider != _myCollider)
            {
                context.Add(collider.transform);
            }
        }

        return context;
    }

    private Vector3 UpdateDirection()
    {
        Vector3 direction = Vector3.zero;
        List<Transform> context = this.GetNearbyEntities();

        for (int i = 0; i < _flockingBehaviors.Length; i++)
        {
            FlockingBehavior behavior = _flockingBehaviors[i];
            direction += behavior.GetDirection(context);
        }

        _direction = direction;
        return direction;
    }
}
