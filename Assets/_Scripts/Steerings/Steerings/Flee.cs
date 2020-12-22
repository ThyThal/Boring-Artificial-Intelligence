using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : ISteeringBehaviour
{
    Transform _myTransform;
    Transform _enemyTransform;
    Rigidbody _myRigidbody;
    float _timePrediction;
    public Flee(Transform myTransform, Transform enemyTransform, Rigidbody myRigidbody, float timePrediction)
    {
        _enemyTransform = enemyTransform;
        _myRigidbody = myRigidbody;
        _myTransform = myTransform;
    }
    public Vector3 GetDirection()
    {
        Vector3 posPrediction = _enemyTransform.position + _timePrediction * _myRigidbody.velocity.magnitude * _enemyTransform.forward;
        Vector3 dir = (_myTransform.position - posPrediction).normalized;
        return dir;
    }
}

