using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingEntity : MonoBehaviour
{
    [SerializeField] private float _flockRadius = 5f;
    [SerializeField] private Vector3 _direction = Vector3.zero;
    [SerializeField] private LayerMask _alliesLayer;
    [SerializeField] private Collider _myCollider;
    [SerializeField] private FlockingBehavior[] _flockingBehaviors;
    public bool isFlocking;


    private void Awake()
    {
        _myCollider = this.GetComponent<Collider>();
        
    }

    private void Start()
    {
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

    public Vector3 UpdateDirection()
    {
        Vector3 direction = Vector3.zero;
        List<Transform> context = this.GetNearbyEntities();

        for (int i = 0; i < _flockingBehaviors.Length; i++)
        {
            FlockingBehavior behavior = _flockingBehaviors[i];
            direction += behavior.GetDirection(context);

            if (float.IsNaN(direction.x))
            {
                Debug.Log("NaN");
            }
        }

        _direction = direction;
        return direction;
    }
}
