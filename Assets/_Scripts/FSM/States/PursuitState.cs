using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private Transform _enemyTransform;
    private Avoid _avoid;
    private float currentDistance;
    public PursuitState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
        _avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
    }

    public override void Awake()
    {
        //Debug.Log("Pursuit State Awake");
        _enemyTransform = _minionController.currentEnemy;
        _avoid.SetTarget(_enemyTransform);
    }

    public override void Execute()
    {
        if (_enemyTransform != null)
        {
            CheckCooldown();
        }

        if (_enemyTransform == null)
        {
            _minionController.CheckEnemies();
            _enemyTransform = _minionController.currentEnemy;
        }
    }

    public override void Sleep()
    {
        
    }

    private void CheckCooldown()
    {
        if (_minionController.currentAttackCooldown <= 0)
        {
            CheckDistance();
        }
    }

    private void CheckDistance()
    {
        currentDistance = (_enemyTransform.position - _minionController.transform.position).magnitude;
        if (currentDistance <= 10) { AttackEnemy(); }
        if (currentDistance > 8)
        {
            _minionController.Move(_avoid.GetDirection());
            _minionController.Look(_enemyTransform.position);
        }
    }

    private void AttackEnemy()
    {
        var getLifeController = _enemyTransform.GetComponent<MinionController>().lifeController;
        if (getLifeController != null)
        {
            getLifeController.GetDamage(_minionController.damage);
        }

        else
        {
            return;
        }

        _minionController.currentAttackCooldown = _minionController.originalAttackCooldown;
    }
}
