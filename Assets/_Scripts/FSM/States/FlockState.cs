using System.Collections.Generic;
using UnityEngine;

public class FlockState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private FlockingEntity _flockingEntity;

    public FlockState(MinionController minionController, FlockingEntity flockingEntity)
    {
        _fsm = minionController.fsm;
        _minionController = minionController;
        _flockingEntity = flockingEntity;

    } // Constructor del Estado.

    public override void Execute()
    {
        Debug.Log("Flocking State Execute");

        // Get Nearby Allies.
        List<Transform> context = _flockingEntity.GetNearbyEntities();

        // If there is none, idle.
        if (context.Count == 0)
        {
            _minionController.fsm.Transition(MinionController.States.IDLE); 
        }

        // If there are allies, flock.
        else
        {
            Vector3 flockingDirection = _flockingEntity.UpdateDirection();
            _minionController.Move(flockingDirection.normalized);
        }
    }

    public override void Awake()
    {
        Debug.Log("Flocking State Awake");
    }

    public override void Sleep()
    {
        Debug.Log("Flocking State Sleep");
    }
}
