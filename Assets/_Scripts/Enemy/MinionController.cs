using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public enum States
    {
        FLOCKING,
        SEARCHING,
        PURSUIT,
        FLEE,
        IDLE
    }

    [Header("Main Stats")]
    [SerializeField] private bool _isBoss;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _speed = 2f;

    [Header("Scripts Components")]
    [SerializeField] private FlockingEntity _flockingEntity;
    [SerializeField] private LineOfSight _lineOfSight;
    [SerializeField] private LifeController _lifeController;

    public FSM<States> _fsm;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        InitializeFSM();
    }

    protected virtual void Update()
    {
        _fsm.OnUpdate();
    }

    private void InitializeFSM()
    {
        // Create Minion FSM.
        _fsm = new FSM<States>();
        IdleState<States> idleState = new IdleState<States>(_fsm, this, _flockingEntity);
        FlockState<States> flockState = new FlockState<States>(_fsm, this, _flockingEntity);

        // Transitions.
        flockState.AddTransition(States.IDLE, idleState);
        idleState.AddTransition(States.FLOCKING, flockState);

        _fsm.SetInitState(idleState);
    }

    public void ExecuteBinaryTree()
    {

    }

    public void Move(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + (direction * _speed * Time.deltaTime);
        transform.position = targetPosition;
    }
}
