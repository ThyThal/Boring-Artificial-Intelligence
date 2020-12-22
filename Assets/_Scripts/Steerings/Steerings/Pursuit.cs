using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : ISteeringBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _myTransform;
    [SerializeField] private Rigidbody _targetRigidbody;
    [SerializeField] private float _timePrediction;

    public Pursuit(Transform myTransform, Transform targetTransform, Rigidbody targetRigidbody, float timePrediction)
    {
        _myTransform = myTransform;
        _targetTransform = targetTransform;
        _targetRigidbody = targetRigidbody;
        _timePrediction = timePrediction;
    }

    public Vector3 GetDirection()
    {
        if (_targetRigidbody != null)
        {
            float velocity = _targetRigidbody.velocity.magnitude;
            Vector3 posPrediction = ((_targetTransform.transform.position) + (_targetTransform.transform.forward * velocity * _timePrediction));
            Vector3 direction = (posPrediction - _myTransform.position).normalized;
            return direction;
        }

        else
        {
            return Vector3.zero;
        }
    }
}
