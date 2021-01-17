using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingEntity : MonoBehaviour
{
    private float _flockRadius = 5f;
    private LayerMask _alliesLayer;
    private Collider _myCollider;
    private FlockingBehavior[] _flockingBehaviors;

    private void Awake()
    {
        _myCollider = this.GetComponent<Collider>();
        _flockingBehaviors = this.GetComponents<FlockingBehavior>();
    }

    private List<Transform> GetNearbyEntities()
    {
        List<Transform> context = new List<Transform>(); // Crea lista a devolver.
        Collider[] contextColliders = Physics.OverlapSphere(this.transform.position, _flockRadius, _alliesLayer); // Obtiene los aliados en un radio.

        foreach (var collider in contextColliders)
        {
            if (collider != _myCollider)
            {
                context.Add(collider.transform); // Añade el transform a la lista.
            }
        }

        return context;
    }

    private Vector3 UpdateDirection()
    {
        Vector3 direction = Vector3.zero;

        for (int i = 0; i < _flockingBehaviors.Length; i++)
        {
            FlockingBehavior behavior = _flockingBehaviors[i];
            direction += behavior.GetDirection();
        }



        return direction;
    }
}
