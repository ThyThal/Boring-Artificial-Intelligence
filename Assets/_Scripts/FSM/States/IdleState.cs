using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private float _timer;

    public IdleState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
    } // Constructor del Estado Idle.

    public override void Awake()
    {
        //Debug.Log("Idle State Awake");
        _timer = Random.Range(1, 3);
    }

    public override void Execute()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }

        else
        {
            // If Minion is BOSS
            if (_minionController.isBoss) { _fsm.Transition(MinionController.States.SEARCHING); }
            // If Minion is NOT BOSS
            else { _fsm.Transition(MinionController.States.FLOCKING); }
        }
    }

    public override void Sleep()
    {
        //Debug.Log("Idle State Sleep");
    }
}
