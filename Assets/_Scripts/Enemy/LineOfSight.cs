using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] public LayerMask obstaclesLayer;

    [SerializeField] public bool sawTarget = false;
    [SerializeField] public Transform target;
    [SerializeField] private float _sightRange = 0;
    [SerializeField] private float _sightAngle = 100;

    private void Update()
    {
        if (target != null)
        {
            IsInSight();
        }

        else
        {
            sawTarget = false;
        }
    }

    public void SetTarget(GameObject gameObject)
    {
        target = gameObject.transform;
    }

    public void IsInSight()
    {
        Vector3 _distanceToTarget = (target.position - transform.position);
        float _distance = _distanceToTarget.magnitude;        

        if (target == null)
        {
            sawTarget = false;
            return;
        } // Check Target.

        if (_distance > _sightRange) 
        {
            sawTarget = false;
            return;
        } // Check Distance to Target.

        float _angleToTarget = Vector3.Angle(transform.forward, _distanceToTarget.normalized);
        if (_angleToTarget > (_sightAngle / 2))
        {
            sawTarget = false;
            return;
        } // Check Angle to Target.

        if (Physics.Raycast(transform.position, _distanceToTarget.normalized, _distance, obstaclesLayer))
        {
            sawTarget = false;
            return;
        } // Check for obstacles.

        sawTarget = true;
    } // Check if Target is on Sight.

    public bool SawTarget()
    {
        return sawTarget;
    } // Return if has Enemy Sight
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * _sightRange);
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _sightAngle / 2, 0) * transform.forward * _sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_sightAngle / 2, 0) * transform.forward * _sightRange);
    } // Draw Sight

}
