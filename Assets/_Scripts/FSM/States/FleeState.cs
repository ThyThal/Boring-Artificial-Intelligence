using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private Flee _flee;
    private Avoid _avoid;

    private float _lowHealth = 25f;
    private float _recoverHealth = 50f;
    private float _OGlowHealth = 25f;
    private float _OGrecoverHealth = 50f;
    private int healMinion = 5;
    private int healBoss = 25;


    private float originalHealCooldown = 1.5f;
    private float currentHealCooldown = 0f;

    public FleeState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
        _OGlowHealth = _lowHealth;
        _OGrecoverHealth = _recoverHealth;
    }

    public override void Awake()
    {
        Debug.Log("Flee State Awake");
        if (_minionController.currentEnemy == null) { _minionController.SelectRandomEnemy(); }

        _flee = new Flee(_minionController.transform, _minionController.currentEnemy.transform, _minionController.GetComponent<Rigidbody>(), 0.5f);
        _avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
        _minionController.isFlee = true;
        _minionController._speed = _minionController._ogSpeed * 1.5f;
        
        

        if (_minionController.isBoss)
        {
            _minionController.AlertFlee();
        }
    }

    public override void Execute()
    {
        var currentLife = _minionController.lifeController.GetCurrentLife();
        currentHealCooldown -= Time.deltaTime;

        if (_minionController.isBoss)
        {
            _lowHealth = _OGlowHealth * 2f;
            _recoverHealth = _OGrecoverHealth * 1.5f;

            if (currentLife >= _recoverHealth) { _minionController.AlertFlee(); }
        }



        if (currentLife < _lowHealth)
        {
            RegenerateLife(); // Regenerate Life
            var fleeDirection = _flee.GetDirection() + _minionController.transform.position;
            _avoid.SetTargetByVector(fleeDirection); // Set flee World Position as Target.
            _minionController.Move(_avoid.GetDirection());
            return;
        } // Recover Health & Flee.
        if (currentLife < _recoverHealth)
        {
            RegenerateLife(); // Regenerate Life
            return;
        } // Recover Health & Not Flee.
        if (currentLife >= _recoverHealth)
        {
            _minionController._speed = _minionController._ogSpeed;
            _fsm.Transition(MinionController.States.IDLE);
        } // Back to Battle.
    }

    private void RegenerateLife()
    {
        if (currentHealCooldown <= 0)
        {
            currentHealCooldown = originalHealCooldown;

            if (_minionController.isBoss) { _minionController.lifeController.GetHeal(healBoss, true); }
            else { _minionController.lifeController.GetHeal(healMinion, false); }
            
        }
    }
}
