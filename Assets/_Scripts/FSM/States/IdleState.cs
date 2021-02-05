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
        Debug.Log("Idle State Awake");
        _timer = Random.Range(1, 1.5f);
        _minionController.isFlee = false;
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
            if (_minionController.isBoss) 
            {
                _fsm.Transition(MinionController.States.SEARCHING); 
            
            }

            // If Minion is NOT BOSS
            else
            {
                if (_minionController.IsBossAlive() == true)
                {
                    _fsm.Transition(MinionController.States.FLOCKING);
                    return;
                }

                else
                {
                    if (_minionController.enemiesMinionList.Count > 0 && _minionController.IsBossAlive() == false)
                    {
                        _fsm.Transition(MinionController.States.PURSUIT);
                    }
                }
                
            }
        }
    }

    public override void Sleep()
    {
        //Debug.Log("Idle State Sleep");
    }
}
