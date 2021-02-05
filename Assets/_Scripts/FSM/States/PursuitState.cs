using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private Transform _enemyTransform;
    private MinionController _enemyController;
    public Avoid avoid;

    private float currentAttackCooldown;
    private float currentDistance;
    public PursuitState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
        currentAttackCooldown = _minionController.originalAttackCooldown;
    }

    public override void Awake()
    {        
        Debug.Log("Pursuit State Awake");
        if (_minionController.IsBossAlive() == false)
        {
            _enemyController = _minionController.SelectRandomEnemy();
            _enemyTransform = _enemyController.transform;
        }

        _enemyTransform = _minionController.currentEnemy;

        if (_enemyTransform == null || _enemyController == null)
        {
            _enemyController = _minionController.SelectRandomEnemy();
            _enemyTransform = _enemyController.transform;
        }

        _enemyController = _enemyTransform.GetComponent<MinionController>();

        avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
        avoid.SetTarget(_enemyTransform);
    }

    public override void Execute()
    {
        // Cooldown
        currentAttackCooldown -= Time.deltaTime;

        if (_enemyTransform != null)
        {
            CheckCooldown();
        }

        if (_enemyTransform == null)
        {
            if (_minionController.lineOfSight.SawTarget() == false)
            {
                _fsm.Transition(MinionController.States.IDLE);
            }
        }
    }

    public override void Sleep()
    {
        /*_enemyTransform = null;
        _enemyController = null;
        _minionController.currentEnemy = null;*/
    }

    private void CheckCooldown()
    {
        if (currentAttackCooldown <= 0)
        {
            CheckDistance();
        }
    }

    private void CheckDistance()
    {
        if (_enemyTransform == null) { return; }
        currentDistance = (_enemyTransform.position - _minionController.transform.position).magnitude;

        if (currentDistance > 8)
        {
            avoid.SetTarget(_minionController.currentEnemy);
            _minionController.Move(avoid.GetDirection());
            _minionController.Look(_enemyTransform.position);

            //Debug.Log($"Enemy Transform: {_enemyTransform.position}");
        }

        if (currentDistance <= 10) { AttackEnemy(); }
    }

    public void SetCurrentTarget(Transform transform)
    {
        avoid.SetTarget(transform);
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

        currentAttackCooldown = _minionController.originalAttackCooldown;
    }
}
