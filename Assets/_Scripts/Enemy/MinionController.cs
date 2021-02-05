using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] public float bossMultiplier = 5f;
    [SerializeField] private bool isGreen;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    [SerializeField] public float _speed = 2f;
    public float _ogSpeed;

    [Header("Low Health & Flee")]
    [SerializeField] public MinionController teamBoss;
    [SerializeField] public bool isFlee;
    public bool goneFlee = false;
    [SerializeField] public float _lowHealth = 25f;
    [SerializeField] public float _recoverHealth = 50;
    [SerializeField] public bool allowRoulette;
    private float _OGlowHealth;


    [Header("Saved Targets")]
    [SerializeField] public List<GameObject> allyMinionsList;
    [SerializeField] public List<GameObject> enemiesMinionList;
    private List<GameObject> enemiesObjects = new List<GameObject>();
    public bool sawEnemy;
    [SerializeField] public Transform currentNode;
    [SerializeField] public Transform currentEnemy;
    [SerializeField] public Vector3 savedEnemyPosition;
    [SerializeField] public Transform currentObstacle;

    [Header("Obstacle Values")]
    [SerializeField] public float obstacleRadius = 2.5f;
    [SerializeField] public float obstacleWeight = 1f;

    [Header("Attack Stats")]
    [SerializeField] public float damage = 15f;
    [SerializeField] public float attackDistace = 3f;

    [Header("Cooldowns")]
    [SerializeField] public float originalAttackCooldown = 3f;
    [SerializeField] public float originalHealCooldown = 1f;

    [Header("Scripts Components")]
    [SerializeField] public LineOfSight lineOfSight;
    [SerializeField] public EnemyMovement enemyMovement;
    [SerializeField] private FlockingEntity _flockingEntity;
    [SerializeField] public LifeController lifeController;
    [SerializeField] private LayerMask wallLayer;


    public FSM<States> fsm;
    private INode _initTree;

    // Roulette Values
    private Roulette<string> _roulette;
    private Dictionary<string, int> _rouletteStates;
    private string _pursuit = "pursuit"; // Only Roulette
    private string _flee = "flee"; // Only Roulette


    protected virtual void Awake()
    {
        // Life Values
        if (isBoss)
        {
            lifeController = new LifeController(_maxHealth * bossMultiplier, Death);
            _lowHealth = _lowHealth * bossMultiplier;
            _recoverHealth = _recoverHealth * bossMultiplier;
        }

        else
        {
            lifeController = new LifeController(_maxHealth, Death);
        }

        _ogSpeed = _speed;        
        _currentHealth = lifeController.GetCurrentLife();
    }

    protected virtual void Start()
    {
        InitializeFSM();
        //InitializeBinaryTree();        
    }

    protected virtual void Update()
    {
        // Life
        _currentHealth = lifeController.GetCurrentLife();

        if (isBoss == false)
        {
            if (LowHealth() == true && allowRoulette == true)
            {
                if (goneFlee == false)
                {
                    RouletteAction(); // Create & Rolls Roulette    
                }
            }

            if (teamBoss.isFlee == true)
            {
                fsm.Transition(States.FLOCKING);
            } // Check if Boss Flees, Minion goes to FLOCKING.

        }

        if (isBoss == true)
        {
            if (isFlee == false && allowRoulette == false)
            {
                CheckLineOfSight();
            }  // Check Line of Sight.

            if (LowHealth() == true && allowRoulette == true)
            {
                RouletteAction(); // Create & Rolls Roulette
            }
        }

        fsm.OnUpdate();
    }

    private void InitializeFSM()
    {
        // Create Minion FSM.
        fsm = new FSM<States>();
        IdleState<States> idleState = new IdleState<States>(fsm, this);
        SearchState<States> searchState = new SearchState<States>(fsm, this);
        FlockState<States> flockState = new FlockState<States>(fsm, this, _flockingEntity);
        PursuitState<States> pursuitState = new PursuitState<States>(fsm, this);
        FleeState<States> fleeState = new FleeState<States>(fsm, this);

        // [EVERYONE] Idle Transitions.
        idleState.AddTransition(States.FLOCKING, flockState);
        idleState.AddTransition(States.SEARCHING, searchState);
        idleState.AddTransition(States.PURSUIT, pursuitState);

        pursuitState.AddTransition(States.FLEE, fleeState);
        pursuitState.AddTransition(States.FLOCKING, flockState);
        pursuitState.AddTransition(States.IDLE, idleState);

        fleeState.AddTransition(States.IDLE, idleState);
        fleeState.AddTransition(States.PURSUIT, pursuitState);


        // [BOSS] Search Transitions.
        searchState.AddTransition(States.IDLE, idleState);
        searchState.AddTransition(States.PURSUIT, pursuitState);

        // [MINIONS] Flocking Transitions.
        flockState.AddTransition(States.IDLE, idleState);
        flockState.AddTransition(States.PURSUIT, pursuitState);

        fsm.SetInitState(idleState);
    }
    
    private void GoToPursuit() { fsm.Transition(MinionController.States.PURSUIT); }
    private void GoToRandomState() { fsm.Transition(ExecuteRoulette()); }

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

    public void CheckLineOfSight()
    {
        for (int i = 0; i < enemiesMinionList.Count; i++)
        {
            var newEnemy = enemiesMinionList[i].GetComponent<MinionController>();

            if (newEnemy != null)
            {
                lineOfSight.SetTarget(newEnemy.gameObject); // Sets current target.
                lineOfSight.IsInSight(); // Runs Line of Sight.

                if (lineOfSight.SawTarget() && teamBoss.isFlee == false) // If saw Target and BOSS not flee.
                {
                    currentEnemy = newEnemy.transform;
                    AlertAllies(); // Transition Allies to Pursuit.
                }
            }
        }
    }

    private void CreateRoulette()
    {
        _roulette = new Roulette<string>();
        var flockComponent = GetComponent<FlockingEntity>();
        List<Transform> nearAllies = flockComponent.GetNearbyEntities();

        _rouletteStates = new Dictionary<string, int>();
        _rouletteStates.Add(_flee, 85);
        _rouletteStates.Add(_pursuit, 15 * nearAllies.Count);
    }
    public MinionController.States ExecuteRoulette()
    {
        var a = _roulette.Run(_rouletteStates);
        if (a == _pursuit) { return States.PURSUIT; }
        else { return States.FLEE; }
    }

    // Main Methods 
    private void Nothing()
    {
        return;
    }
    public void Move(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + (direction * _speed * Time.deltaTime);
        var hit = Physics.Raycast(transform.position, transform.forward, 3f, wallLayer);
        if (hit == true) { return; }
        transform.position = targetPosition;
    }
    public void Look(Vector3 point)
    {
        transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
    }
    public void Death()
    {
        if (isGreen)
        { 
            GameManager.Instance.greenKilledAmount -= 1;
            GameManager.Instance.levelController.greenTeamList.Remove(this.gameObject);
        }
        else 
        { 
            GameManager.Instance.orangeKilledAmount -= 1;
            GameManager.Instance.levelController.orangeTeamList.Remove(this.gameObject);
        }

        Destroy(this.gameObject);
    }

    // Extra Methods
    public bool IsBossAlive()
    {
        if (teamBoss != null) { return true; }
        else return false;
    }
    private void AlertAllies()
    {
        for (int i = 0; i < allyMinionsList.Count; i++)
        {
            var currentAlly = allyMinionsList[i].GetComponent<MinionController>(); // Create Ally Reference
            if (currentAlly == null) { return; } // No Allies.

            if (currentAlly.currentEnemy == null)
            {
                currentAlly.currentEnemy = currentEnemy; // Sets all minions enemy to Boss enemy.
                currentAlly.GoToPursuit();
            }

            else
            {
                currentAlly.GoToPursuit();
            }
        }
    }
    public void AlertFlee()
    {
        for (int i = 0; i < allyMinionsList.Count; i++)
        {
            var currentAlly = allyMinionsList[i].GetComponent<MinionController>(); // Create Ally Reference

            if (currentAlly == null) { return; } // No Allies.

            if (currentAlly != this && currentAlly.isBoss == false) // If not Boss, FLOCK.
            {
                currentAlly.currentEnemy = null; // Removes Current Enemy
                currentAlly.fsm.Transition(States.FLOCKING); // Goes To Flocking
            }
        }
    }
    public MinionController SelectRandomEnemy()
    {
        var random = Random.Range(0, enemiesMinionList.Count);
        return enemiesMinionList[random].GetComponent<MinionController>();
    }

    public void Kamikaze()
    {
        if (isBoss == true && goneFlee == true)
        {
            fsm.Transition(MinionController.States.PURSUIT);
        }
    }

    private bool LowHealth()
    {
        if (_currentHealth < _lowHealth)
        {
            if (goneFlee == true)
            {
                allowRoulette = false;
            }

            else
            {
                allowRoulette = true;
            }

            return true;
        }

        else
        {
            return false;
        }
    }
    private bool RecoverHealth()
    {
        if (_currentHealth < _recoverHealth)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
    private void RouletteAction()
    {
        CreateRoulette();
        allowRoulette = false;
        fsm.Transition(ExecuteRoulette());
    }
}
