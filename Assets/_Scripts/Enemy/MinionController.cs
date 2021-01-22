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
    [SerializeField] public bool isBoss;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _speed = 2f;

    [Header("Saved Targets")]
    [SerializeField] public Transform currentNode;
    [SerializeField] public Transform currentEnemy;
    [SerializeField] public Transform currentObstacle;

    [Header("Obstacle Values")]
    [SerializeField] public float obstacleRadius = 2.5f;
    [SerializeField] public float obstacleWeight = 1f;

    [Header("Scripts Components")]
    [SerializeField] public LineOfSight lineOfSight;
    [SerializeField] public EnemyMovement enemyMovement;
    [SerializeField] private FlockingEntity _flockingEntity;
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
        IdleState<States> idleState = new IdleState<States>(_fsm, this);
        SearchState<States> searchState = new SearchState<States>(_fsm, this);
        FlockState<States> flockState = new FlockState<States>(_fsm, this, _flockingEntity);

        // [EVERYONE] Idle Transitions.
        idleState.AddTransition(States.FLOCKING, flockState);
        idleState.AddTransition(States.SEARCHING, searchState);

        // [BOSS] Search Transitions.
        searchState.AddTransition(States.IDLE, idleState);

        // [MINIONS] Flocking Transitions.
        flockState.AddTransition(States.IDLE, idleState);

        _fsm.SetInitState(idleState);
    }

    public void ExecuteBinaryTree()
    {

    }

    public void ExecuteTreeFromSleep()
    {

    }

    public void Move(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + (direction * _speed * Time.deltaTime);
        transform.position = targetPosition;
    }

    public void Look(Vector3 point)
    {
        transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
    }
}
