using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float originalSpeed;
    [SerializeField] public float safeDistanceToEnemy = 5f;
    [SerializeField] public float safeDistanceToWaypoint = 1.3f;

    [SerializeField] private Rigidbody _rigidbody;

    private void Awake()
    {
        originalSpeed = speed;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        var moveVelocity = dir * speed;
        _rigidbody.velocity = moveVelocity;
    }

    public void Look(Vector3 point)
    {
        transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
    }

}
