using System;
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
    [SerializeField] private bool isGreen;
    [SerializeField] public GameObject[] minionsList;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _speed = 2f;

    [Header("Saved Targets")]
    private GameObject[] _enemiesList;
    public bool sawEnemy;
    [SerializeField] public Transform currentNode;
    [SerializeField] public Transform currentEnemy;
    [SerializeField] public Transform currentObstacle;

    [Header("Obstacle Values")]
    [SerializeField] public float obstacleRadius = 2.5f;
    [SerializeField] public float obstacleWeight = 1f;

    [Header("Attack Stats")]
    [SerializeField] public float damage = 15f;
    [SerializeField] public float attackDistace = 3f;

    [Header("Cooldowns")]
    [SerializeField] public float currentHealCooldown = 3f;
    [SerializeField] public float currentAttackCooldown = 1f;
    public float originalAttackCooldown;
    public float originalHealCooldown;

    [Header("Scripts Components")]
    [SerializeField] public LineOfSight lineOfSight;
    [SerializeField] public EnemyMovement enemyMovement;
    [SerializeField] private FlockingEntity _flockingEntity;
    [SerializeField] public LifeController lifeController;


    public FSM<States> _fsm;
    private INode _initTree;

    // Roulette Values
    private Roulette<string> _roulette;
    private Dictionary<string, int> _rouletteStates;
    private string _pursuit = "pursuit"; // Only Roulette
    private string _flee = "flee"; // Only Roulette


    protected virtual void Awake()
    {
        if (isGreen) {
            minionsList = GameObject.FindGameObjectsWithTag("GreenEnemy");
            _enemiesList = GameObject.FindGameObjectsWithTag("OrangeEnemy"); }

        else {
            minionsList = GameObject.FindGameObjectsWithTag("OrangeEnemy");
            _enemiesList = GameObject.FindGameObjectsWithTag("GreenEnemy"); }

        // Original Values
        originalHealCooldown = currentHealCooldown;
        originalAttackCooldown = currentAttackCooldown;
        lifeController = new LifeController(_maxHealth, Death);
    }

    protected virtual void Start()
    {
        CreateRoulette();
        InitializeFSM();
        InitializeBinaryTree();
    }

    protected virtual void Update()
    {
        // Timers
        currentHealCooldown -= Time.deltaTime;
        currentAttackCooldown -= Time.deltaTime;
        _fsm.OnUpdate();
        CheckEnemies();
    }

    private void InitializeFSM()
    {
        // Create Minion FSM.
        _fsm = new FSM<States>();
        IdleState<States> idleState = new IdleState<States>(_fsm, this);
        SearchState<States> searchState = new SearchState<States>(_fsm, this);
        FlockState<States> flockState = new FlockState<States>(_fsm, this, _flockingEntity);
        PursuitState<States> pursuitState = new PursuitState<States>(_fsm, this);

        // [EVERYONE] Idle Transitions.
        idleState.AddTransition(States.FLOCKING, flockState);
        idleState.AddTransition(States.SEARCHING, searchState);
        idleState.AddTransition(States.PURSUIT, pursuitState);

        // [BOSS] Search Transitions.
        searchState.AddTransition(States.IDLE, idleState);
        searchState.AddTransition(States.PURSUIT, pursuitState);

        // [MINIONS] Flocking Transitions.
        flockState.AddTransition(States.IDLE, idleState);
        flockState.AddTransition(States.PURSUIT, pursuitState);

        _fsm.SetInitState(idleState);
    }
    private void GoToIdle() { _fsm.Transition(MinionController.States.IDLE); }
    private void GoToSearch() { _fsm.Transition(MinionController.States.SEARCHING); }
    private void GoToFlocking() { _fsm.Transition(MinionController.States.FLOCKING); }
    private void GoToPursuit() { _fsm.Transition(MinionController.States.PURSUIT); }
    private void GoToFlee() { _fsm.Transition(MinionController.States.FLEE); }
    private void GoToRandomState() { _fsm.Transition(ExecuteRoulette()); }

    private void InitializeBinaryTree()
    {
        ActionNode _actionPursuit = new ActionNode(GoToPursuit);
        ActionNode _actionRandomState = new ActionNode(GoToRandomState);
        ActionNode _actionNothing = new ActionNode(Nothing);

        QuestionNode _questionSight = new QuestionNode(lineOfSight.SawTarget, _actionPursuit, _actionNothing);
        _initTree = _questionSight;
    }

    public void ExecuteBinaryTree()
    {
        _initTree.Execute();
    }

    public void ExecuteTreeFromSleep()
    {
        _initTree.Execute();
    }

    #region
    private void CreateRoulette()
    {
        _roulette = new Roulette<string>();

        _rouletteStates = new Dictionary<string, int>();
        _rouletteStates.Add(_flee, 80);
        _rouletteStates.Add(_pursuit, 20);
    }

    MinionController.States ExecuteRoulette()
    {
        var a = _roulette.Run(_rouletteStates);
        if (a == _pursuit) { return States.PURSUIT; }
        else { return States.FLEE; }
    }
    #endregion === ROULETTE ===


    public void CheckEnemies()
    {
        if (currentEnemy == null)
        {
            foreach (var enemy in _enemiesList)
            {
                if (enemy != null)
                {
                    lineOfSight.SetTarget(enemy);
                    currentEnemy = enemy.transform;
                    lineOfSight.IsInSight();
                }

                if (lineOfSight.SawTarget() && enemy != null)
                {
                    AlertAllies();
                    return;
                }
            }
        }

        if (currentEnemy != null)
        {
            if (lineOfSight.SawTarget())
            {
                AlertAllies();
                return;
            }
        }

    }

    private void AlertAllies()
    {
        foreach (var ally in minionsList)
        {
            if (ally == null) { return; }
            var controller = ally.GetComponent<MinionController>();
            controller.currentEnemy = currentEnemy;
            controller.GoToPursuit();
        }
    }
    private void Nothing()
    {
        return;
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
    public void Death()
    {
        if (isGreen) { GameManager.Instance.greenKilledAmount -= 1; }
        else { GameManager.Instance.orangeKilledAmount -= 1; }
        Destroy(this.gameObject);
    }

    public void GenerateLists()
    {
        if (isGreen)
        {
            minionsList = GameObject.FindGameObjectsWithTag("GreenEnemy");
            _enemiesList = GameObject.FindGameObjectsWithTag("OrangeEnemy");
        }

        else
        {
            minionsList = GameObject.FindGameObjectsWithTag("OrangeEnemy");
            _enemiesList = GameObject.FindGameObjectsWithTag("GreenEnemy");
        }
    }


}
