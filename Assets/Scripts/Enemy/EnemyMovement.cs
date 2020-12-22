using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float _speed;
    [SerializeField] public float _originalSpeed;
    [SerializeField] private Rigidbody _rigidbody;

    private void Awake()
    {
        _originalSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        var moveVelocity = dir * _speed;
        _rigidbody.velocity = moveVelocity;
    }

    public void Look(Vector3 point)
    {
        transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
    }

}
