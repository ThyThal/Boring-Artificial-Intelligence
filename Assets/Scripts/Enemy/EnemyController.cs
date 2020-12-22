using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Referencia al FSM.
    [SerializeField] private FSM<string> _fsm;
    [SerializeField] private SleepState<string> _sleepState;
    [SerializeField] private FleeState<string> _fleeState;
    [SerializeField] private INode _initTree;
    [SerializeField] private bool isBoss;

    // Keys de los estados del FSM.
    private const string _idle = "idle";
    private const string _search = "search";
    private const string _flee = "flee";
    private const string _pursuit = "pursuit";
    private const string _sleep = "sleep";

    [Header("Health Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool _lowHealth = false;
    [SerializeField] private float _healCooldown;
    [SerializeField] private float _ogHealCooldown;

    [Header("Attack Stats")]
    [SerializeField] public float damage = 50f;
    [SerializeField] public float distanceToTarget = 5;
    [SerializeField] public float currentDistanceTarget;
    [SerializeField] public float attackCooldown = 2f;
    [SerializeField] public float ogAttackCooldown;


    // Target Variables.
    [Header("Target Info")]
    [SerializeField] public float currentWaypointDistance;
    [SerializeField] private List<Node> _nodes;
    [SerializeField] public Node targetNode;
    [SerializeField] private bool reachedWaypoint;
    [SerializeField] public float distanceToWaypoint;
    [SerializeField] private Transform _targetEnemy;
    [SerializeField] private bool _sawTarget;

    // Componentes.
    [Header("Componentes")]
    [SerializeField] public LineOfSight lineOfSight;
    [SerializeField] public LifeController lifeController;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] public LayerMask obstacleLayer;
    [SerializeField] public float obstacleRadius = 1f;
    [SerializeField] public float obstacleWeight = 1f;
    [SerializeField] private Roulette<string> _roulette;
    [SerializeField] private Dictionary<string, int> _rouletteStates;

    private void Awake()
    {
        _ogHealCooldown = _healCooldown;
        ogAttackCooldown = attackCooldown;
        lifeController = new LifeController(maxHealth, Death);
        currentHealth = lifeController.GetCurrentLife();
        _rigidbody = GetComponent<Rigidbody>();
        lineOfSight = GetComponent<LineOfSight>();
        _enemyMovement = GetComponent<EnemyMovement>();

        CreateRoulette();
        InitFSM();
        InitBinaryTree();
    }
    private void Update()
    {
        _healCooldown -= Time.deltaTime;
        attackCooldown -= Time.deltaTime;
        currentHealth = lifeController.GetCurrentLife();
        _fsm.OnUpdate();        

        if (targetNode != null)
        {
            HasReachedWaypoint();
        }

        if (lineOfSight.SawTarget())
        {
            GoToPursuit();
        }

        if (currentHealth <= 20)
        {
            _lowHealth = true;
            GoToRandomState();
        }

        if (_lowHealth && currentHealth <= 40)
        {
            var currentState = _fsm.GetCurrentState();
            if (_fsm.GetCurrentState() == _fleeState)
            {
                RegenerateLife();
            }

            if (currentHealth >= 40)
            {
                _lowHealth = false;
                GoToIdle();
            }            
        }
    }

    public void Move(Vector3 dir)
    {
        _enemyMovement.Move(dir);
    }
    public void Look(Vector3 position)
    {
        _enemyMovement.Look(position);
    }

    // === FSM METHODS===
    #region === FSM METHODS===
    private void InitFSM()
    {
        // Inicializar FSM.
        _fsm = new FSM<string>();

        // Crear los estados del FSM.
        IdleState<string> idleState = new IdleState<string>(this);
        PursuitState<string> pursuitState = new PursuitState<string>(_targetEnemy, this);
        SearchState<string> searchState = new SearchState<string>(_nodes, transform, obstacleLayer, this, _targetEnemy);
        _fleeState = new FleeState<string>(this, _targetEnemy, _rigidbody);
        _sleepState = new SleepState<string>(this);

        idleState.AddTransition(_sleep, _sleepState);
        idleState.AddTransition(_search, searchState);
        idleState.AddTransition(_pursuit, pursuitState);
        idleState.AddTransition(_flee, _fleeState);

        searchState.AddTransition(_sleep, _sleepState);
        searchState.AddTransition(_idle, idleState);
        searchState.AddTransition(_pursuit, pursuitState);
        searchState.AddTransition(_flee, _fleeState);

        
        _fleeState.AddTransition(_idle, idleState);/*
        _fleeState.AddTransition(_sleep, _sleepState);
        fleeState.AddTransition(_search, searchState);
        fleeState.AddTransition(_pursuit, pursuitState);
        */

        pursuitState.AddTransition(_sleep, _sleepState);
        pursuitState.AddTransition(_search, searchState);
        pursuitState.AddTransition(_idle, idleState);
        pursuitState.AddTransition(_flee, _fleeState);

        _sleepState.AddTransition(_idle, idleState);
        _sleepState.AddTransition(_search, searchState);
        _sleepState.AddTransition(_pursuit, pursuitState);
        _sleepState.AddTransition(_flee, _fleeState);


        // Asignar un valor inicial.
        _fsm.SetInitState(idleState);
    }
    private void GoToPursuit() {_fsm.Transition(_pursuit); }
    private void GoToFlee() { _fsm.Transition(_flee); }
    private void GoToIdle() {_fsm.Transition(_idle); }
    private void GoToSearchState() {_fsm.Transition(_search); }
    private void GoToRandomState() {_fsm.Transition(ExecuteRoulette()); }

    #endregion  === FSM METHODS===

    // === BINARY TREE METHODS ===
    #region  === BINARY TREE METHODS ===
    public void ExecuteBinaryTree()
    {
        if (_fsm.GetCurrentState() == _sleepState)
        {
            return;
        }

        else
        {
            _initTree.Execute();
        }
    }
    public void ExecuteTreeFromSleep()
    {
        _initTree.Execute();
    }
    private void InitBinaryTree()
    {
        ActionNode _pursuitEnemy = new ActionNode(GoToPursuit);
        ActionNode _idle = new ActionNode(GoToIdle);
        ActionNode _search = new ActionNode(GoToSearchState);
        ActionNode _flee = new ActionNode(GoToFlee);
        ActionNode _randomState = new ActionNode(GoToRandomState);

        QuestionNode _isInSight = new QuestionNode(lineOfSight.SawTarget, _pursuitEnemy, _search);
        QuestionNode _hasReachedWaypoiny = new QuestionNode(() => reachedWaypoint, _idle, _isInSight);
        _initTree = _isInSight;
    } // Binary Tree Decisions.
    #endregion === BINARY TREE METHODS ===

    private bool HasReachedWaypoint()
    {
        var dist = (targetNode.transform.position - transform.position).magnitude;
        if (dist <= distanceToWaypoint)
        {
            reachedWaypoint = true;
            GoToIdle();
            return  true;
        }

        else
        {
            reachedWaypoint = false;
            return false;
        }
    }

    #region
    private void CreateRoulette()
    {
        _roulette = new Roulette<string>();

        _rouletteStates = new Dictionary<string, int>();
        _rouletteStates.Add(_flee, 80);
        _rouletteStates.Add(_pursuit, 20);
    }

    string ExecuteRoulette()
    {
        var a = _roulette.Run(_rouletteStates);
        Debug.Log(a);
        return a;
    }
    #endregion === ROULETTE ===

    private void RegenerateLife()
    {
        if (_healCooldown <= 0)
        {
            _healCooldown = _ogHealCooldown;
            lifeController.GetHeal(5);
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

}
