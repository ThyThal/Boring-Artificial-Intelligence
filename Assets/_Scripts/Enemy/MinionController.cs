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

    public FSM<States> fsm;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        InitializeFSM();
    }

    protected virtual void Update()
    {
        fsm.OnUpdate();
    }

    private void InitializeFSM()
    {
        // Create Minion FSM.
        fsm = new FSM<States>();

        FlockState<States> flockState = new FlockState<States>(this, _flockingEntity);
        //SearchState<States> searchState = new SearchState<States>();
        //PursuitState<States> pursuitState = new PursuitState<States>();
        IdleState<States> idleState = new IdleState<States>(this);

        // [MINIONS] Flocking Transitions.
        flockState.AddTransition(States.FLOCKING, idleState);

        // [BOSS] Searching Transitions.
        //searchState.AddTransition(States.SEARCHING, idleState);

        // [EVERYONE] Pursuit Transitions.
        //pursuitState.AddTransition(States.PURSUIT, idleState);

        // [EVERYONE] Idle Transitions.
        idleState.AddTransition(States.IDLE, flockState);
        //idleState.AddTransition(States.IDLE, searchState);
        //idleState.AddTransition(States.IDLE, pursuitState);


        fsm.SetInitState(idleState);
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
