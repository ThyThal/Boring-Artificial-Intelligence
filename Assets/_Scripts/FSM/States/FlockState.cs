using System.Collections.Generic;
using UnityEngine;

public class FlockState<T> : FSMState<T>
{
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;
    private FlockingEntity _flockingEntity;
    private Avoid _avoid;

    public FlockState(FSM<MinionController.States> fsm, MinionController minionController, FlockingEntity flockingEntity)
    {
        _fsm = fsm;
        _minionController = minionController;
        _flockingEntity = flockingEntity;
        _avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
    } // Constructor del Estado Flock.

    public override void Awake()
    {
        Debug.Log("Flocking State Awake");
        if (_minionController.isBoss == true) { return; }
    }

    public override void Execute()
    {
        // Get Nearby Allies.
        List<Transform> context = _flockingEntity.GetNearbyEntities();

        // If there is none, idle.
        if (context.Count == 0)
        {
            _fsm.Transition(MinionController.States.IDLE); 
        }

        // If there are allies, flock.
        else
        {
            Vector3 flockingDirection = _flockingEntity.UpdateDirection();
            Vector3 worldPositionFlock = flockingDirection + _minionController.transform.position;
            //_minionController.Move(flockingDirection.normalized);

            _avoid.SetTargetByVector(worldPositionFlock);
            _minionController.Move(_avoid.GetDirection());
            _minionController.Look(worldPositionFlock);
        }
    }

    public override void Sleep()
    {
        //Debug.Log("Flocking State Sleep");
    }
}
