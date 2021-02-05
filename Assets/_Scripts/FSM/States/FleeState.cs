using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private float fleeTimer = 6f;
    private Flee _flee;
    private Avoid _avoid;

    // Timers
    private float currentHealCooldown;
    private float originalHealCooldown;

    public FleeState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
        originalHealCooldown = _minionController.originalHealCooldown;
        currentHealCooldown = originalHealCooldown;
    }

    public override void Awake()
    {
        Debug.Log("Flee State Awake");

        _flee = new Flee(_minionController.transform, _minionController.currentEnemy.transform, _minionController.GetComponent<Rigidbody>(), 0.5f);
        _avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
        _minionController.isFlee = true; // Trigger Flee Bool.
        _minionController._speed = _minionController._ogSpeed * 1.5f;
        _minionController.goneFlee = true;
        fleeTimer = 6f;

        if (_minionController.isBoss)
        {
            _minionController.AlertFlee();
        }
    }

    public override void Execute()
    {
        fleeTimer -= Time.deltaTime;
        currentHealCooldown -= Time.deltaTime;
        CheckHealth();
    }

    private void CheckHealth()
    {
        var currentLife = _minionController.lifeController.GetCurrentLife(); // Gets Current Life.

        CheckHealCooldown(); // Check Heal Cooldown

        if (currentLife < _minionController._lowHealth) // If Minion is Low Health
        {
            var fleeDirection = _flee.GetDirection() + _minionController.transform.position;
            _avoid.SetTargetByVector(fleeDirection); // Set flee World Position as Target
            _minionController.Move(_avoid.GetDirection()); // Move to Flee Position
        } // Recover Health & Flee.

        if (currentLife < _minionController._recoverHealth)
        {
            if (_minionController.isBoss) 
            { 
                _minionController.AlertFlee();

                if (fleeTimer <= 0)
                {
                    _minionController.Kamikaze();
                }
            }                
        } // Recover Health & Not Flee.

        if (currentLife >= _minionController._recoverHealth)
        {
            _minionController._speed = _minionController._ogSpeed;
            _fsm.Transition(MinionController.States.IDLE);
        } // Back to Battle.
    }

    private void CheckHealCooldown()
    {
        if (currentHealCooldown <= 0)
        {
            RegenerateLife();
            currentHealCooldown = originalHealCooldown;
        }
    } // Cooldown

    private void RegenerateLife()
    {
        if (_minionController.isBoss)
        {
            _minionController.lifeController.GetHeal(25, true);
        }

        else
        {
            _minionController.lifeController.GetHeal(5, false);
        }
    } // Get Heal

    public override void Sleep()
    {
        _minionController.isFlee = false; // Resets to not Flee.
    }
}
