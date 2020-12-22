using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] public bool _sawTarget = false;
    [SerializeField] public Transform target;
    [SerializeField] private float _sightRange = 0;
    [SerializeField] private float _sightAngle = 100;
    [SerializeField] public LayerMask obstaclesLayer;

    private void Update()
    {
        if (target != null)
        {
            IsInSight();
        }

        else
        {
            _sawTarget = false;
        }
    }

    public void IsInSight()
    {
        Vector3 _distanceToTarget = (target.position - transform.position);
        float _distance = _distanceToTarget.magnitude;        

        if (target == null)
        {
            _sawTarget = false;
            return;
        } // Check Target.

        if (_distance > _sightRange) 
        {
            _sawTarget = false;
            return;
        } // Check Distance to Target.

        float _angleToTarget = Vector3.Angle(transform.forward, _distanceToTarget.normalized);
        if (_angleToTarget > (_sightAngle / 2))
        {
            _sawTarget = false;
            return;
        } // Check Angle to Target.

        if (Physics.Raycast(transform.position, _distanceToTarget.normalized, _distance, obstaclesLayer))
        {
            _sawTarget = false;
            return;
        } // Check for obstacles.

        _sawTarget = true;
    } // Check if Target is on Sight.

    public bool SawTarget()
    {
        return _sawTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * _sightRange);
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _sightAngle / 2, 0) * transform.forward * _sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_sightAngle / 2, 0) * transform.forward * _sightRange);
    } // Draw Sight.

}
