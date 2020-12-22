using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState<T> : FSMState<T>
{
    [SerializeField] private Transform _myTransform;
    [SerializeField] private Transform _target;
    [SerializeField] private Rigidbody _targetRigidbody;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private Pursuit _pursuit;
    [SerializeField] private Avoid _avoid;


    public PursuitState(Transform target, EnemyController enemyController)
    {
        _target = target;
        _myTransform = enemyController.transform;
        _enemyController = enemyController;
        _targetRigidbody = target.GetComponent<Rigidbody>();
        _pursuit = new Pursuit(_enemyController.transform, _target, _targetRigidbody, 0.5f);
        _avoid = new Avoid(_myTransform, _enemyController.obstacleLayer, _enemyController.obstacleRadius, _enemyController.obstacleWeight);
    }

    public override void Awake()
    {

    }
    public override void Sleep()
    {
        
    }

    public override void Execute()
    {
        if (_target == null)
        {
            _enemyController.ExecuteBinaryTree();
        }

        if (_target != null)
        {
            CheckAttackDistance();

            if (_enemyController.currentDistanceTarget > _enemyController.distanceToTarget)
            {
                Vector3 positionDestination = _pursuit.GetDirection();
                _enemyController.Move(positionDestination);
                _enemyController.Look(_target.position);
            }


            Vector3 difference = _target.position - _enemyController.transform.position;
            float distance = difference.magnitude;

            bool obstacle = Physics.Raycast(_enemyController.transform.position, difference.normalized, distance, _enemyController.lineOfSight.obstaclesLayer);
            if (obstacle || distance < 2)
            {
                if (obstacle)
                {
                    _avoid.SetTarget(_target.transform);
                }

                _enemyController.ExecuteBinaryTree();
            }
        }
    }

    private void CheckAttackDistance()
    {
        Debug.LogWarning("DISTANCE");
        _enemyController.currentDistanceTarget = (_target.position - _enemyController.transform.position).magnitude;

        if (_enemyController.currentDistanceTarget <= _enemyController.distanceToTarget)
        {
            if (_enemyController.attackCooldown <= 0)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        Debug.LogWarning("ATTACK");
        _enemyController.attackCooldown = _enemyController.ogAttackCooldown;
        var getLifeController = _target.gameObject.GetComponent<EnemyController>().lifeController;

        if (getLifeController != null)
        {
            getLifeController.GetDamage(_enemyController.damage);
        }

        else
        {
            return;
        }
    }

}
