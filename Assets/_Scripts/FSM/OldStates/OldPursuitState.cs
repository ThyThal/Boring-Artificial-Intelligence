using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPursuitState<T> : FSMState<T>
{
    [SerializeField] private Transform _myTransform;
    [SerializeField] private Transform _enemyTransform;
    [SerializeField] private Rigidbody _enemyRigidbody;
    [SerializeField] private EnemyController _myController;
    [SerializeField] private EnemyMovement _myMovement;
    [SerializeField] private Pursuit _pursuit;
    [SerializeField] private Avoid _avoid;
    private float currentDistance;

    public OldPursuitState(Transform enemyTransform, EnemyController myController)
    {
        _enemyTransform = enemyTransform;
        _myController = myController;
        _myTransform = _myController.transform;
        _enemyRigidbody = enemyTransform.GetComponent<Rigidbody>();
        //_pursuit = new Pursuit(_myTransform, _enemyTransform, _enemyRigidbody, 0.5f);
        _avoid = new Avoid(_myTransform, _myController.obstaclesLayer, _myController.obstacleRadius, _myController.obstacleWeight);
    }

    public override void Awake()
    {
        _avoid.SetTarget(_enemyTransform);
    }

    public override void Sleep()
    {

    }

    public override void Execute()
    {
        if (_enemyTransform == null)
        {
            _myController.ExecuteBinaryTree();
        }

        if (_enemyTransform != null)
        {
            CheckAttackDistance();

            if (currentDistance > 8)
            {
                _myController.Move(_avoid.GetDirection());
                _myController.Look(_enemyTransform.position);
            }
        }
    }

    private void CheckAttackDistance()
    {
        currentDistance = (_enemyTransform.position - _myTransform.position).magnitude; // Get current distance to enemy;
        if (currentDistance <= 10) // Check if it's in attack range, keep moving.
        {
            CheckAttackCooldown();
        }

    }

    private void CheckAttackCooldown()
    {
        if (_myController.currentAttackCooldown <= 0)
        {
            _myController.currentAttackCooldown = _myController.originalAttackCooldown;
            var getLifeController = _enemyTransform.GetComponent<EnemyController>().lifeController;
            if (getLifeController != null)
            {
                getLifeController.GetDamage(_myController.damage);
            }

            else
            {
                return;
            }
        }
    }
}
