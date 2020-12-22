using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] private bool _isBoss;
    [SerializeField] public LayerMask enemyLayer;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _lowHealth = false;
    

    [Header("Saved Targets")]
    [SerializeField] public Transform currentNode;
    [SerializeField] public Transform currentEnemy;
    [SerializeField] public Transform currentObstacle;

    [Header("Attack Stats")]
    [SerializeField] public float damage = 50f;
    [SerializeField] public float attackDistace = 3f;

    [Header("Line of Sight")]
    [SerializeField] public LayerMask obstaclesLayer;
    [SerializeField] public float obstacleRadius = 1.5f;

    [Header("Cooldowns")]
    [SerializeField] public float currentHealCooldown = 3f;
    [SerializeField] public float currentAttackCooldown = 1f;
    [SerializeField] private bool reachedWaypoint;

    #region === Non Serialized Values ===
    // General
    private List<Node> _nodesList;

    // My Components
    public LineOfSight lineOfSight;
    public EnemyMovement enemyMovement;
    public LifeController lifeController;
    private Rigidbody rigidbody;

    // Referencia al FSM.
    private FSM<string> _fsm;
    private SleepState<string> _sleepState;
    private FleeState<string> _fleeState;
    private INode _initTree;

    // Keys de los estados del FSM.
    private const string _idle = "idle";
    private const string _search = "search";
    private const string _flee = "flee";
    private const string _pursuit = "pursuit";
    private const string _sleep = "sleep";

    // Original Values
    public float originalAttackCooldown;
    public float originalHealCooldown;

    // Roulette Values
    private Roulette<string> _roulette;
    private Dictionary<string, int> _rouletteStates;
    #endregion


    private void Awake()
    {
        // Original Values
        originalHealCooldown = currentHealCooldown;
        originalAttackCooldown = currentAttackCooldown;
        _nodesList = GameManager.Instance.nodesList;

        // Components References
        rigidbody = GetComponent<Rigidbody>(); 
        lineOfSight = GetComponent<LineOfSight>();
        enemyMovement = GetComponent<EnemyMovement>();
        lifeController = new LifeController(_maxHealth, Death);

        // Set Max Health
        _currentHealth = lifeController.GetCurrentLife();
        lineOfSight.target = currentEnemy;

        CreateRoulette();
        InitFSM();
        InitBinaryTree();
    }
    private void Update()
    {
        // Timers
        currentHealCooldown -= Time.deltaTime;
        currentAttackCooldown -= Time.deltaTime;
        Debug.Log(_fsm.GetCurrentState());
        
        _currentHealth = lifeController.GetCurrentLife(); // Update Life
        _fsm.OnUpdate(); // Update FSM  

        HasReachedWaypoint(); // Devolver si llegamos al nodo.

        if (lineOfSight.SawTarget())
        {
            GoToPursuit();
        } // SAW TARGET

        if (_currentHealth <= 20)
        {
            _lowHealth = true;
            GoToRandomState();
        } // LOW HEALTH

        if (_lowHealth && _currentHealth <= 40) // LOW HEALTH AFTER
        {
            var currentState = _fsm.GetCurrentState();
            if (_fsm.GetCurrentState() == _fleeState)
            {
                RegenerateLife();
            }

            if (_currentHealth >= 40)
            {
                _lowHealth = false;
                GoToIdle();
            }            
        }
    }

    public void Move(Vector3 dir)
    {
        enemyMovement.Move(dir);
    } // Move Direction
    public void Look(Vector3 position)
    {
        enemyMovement.Look(position);
    } // Look Direction

    // === FSM METHODS===
    #region === FSM METHODS===
    private void InitFSM()
    {
        // Inicializar FSM.
        _fsm = new FSM<string>();

        // Crear los estados del FSM.
        IdleState<string> idleState = new IdleState<string>(this);
        PursuitState<string> pursuitState = new PursuitState<string>(currentEnemy, this);
        SearchState<string> searchState = new SearchState<string>(_nodesList, transform, obstaclesLayer, this, currentEnemy);
        _fleeState = new FleeState<string>(this, currentEnemy, rigidbody);
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
        if (currentNode != null)
        {
            var dist = (currentNode.position - transform.position).magnitude;
            if (dist <= enemyMovement.safeDistanceToWaypoint)
            {
                GoToIdle();
                reachedWaypoint = true;
                return true;
            }

            else
            {
                reachedWaypoint = false;
                return false;
            }            
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
        if (currentHealCooldown <= 0)
        {
            currentHealCooldown = originalHealCooldown;
            lifeController.GetHeal(5);
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

}
