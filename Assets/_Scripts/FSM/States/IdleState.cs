using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    [SerializeField] private MinionController _minionController;
    [SerializeField] private FlockingEntity _flockingEntity;
    [SerializeField] private float _timer;


    [SerializeField] private EnemyController _enemyController;

    public IdleState(EnemyController enemyController)
    {
        _enemyController = enemyController;
    } // Constructor del Estado.

    public IdleState(FSM<MinionController.States> fsm, MinionController minionController, FlockingEntity flockingEntity)
    {
        _fsm = fsm;
        _minionController = minionController;
        _flockingEntity = flockingEntity;
    } // Constructor del Estado.

    public override void Execute()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }

        else
        {
            _fsm.Transition(MinionController.States.FLOCKING);
        }
    }

    public override void Awake()
    {
        Debug.Log("Idle State Awake");
        _timer = Random.Range(1, 3);
    }

    public override void Sleep()
    {
        Debug.Log("Idle State Sleep");
    }
}
