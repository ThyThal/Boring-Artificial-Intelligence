using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState<T> : FSMState<T>
{
    // Constructor Variables.
    private FSM<MinionController.States> _fsm;
    private MinionController _minionController;

    // Extra Variables.
    private GameObject[] _minionsList;
    private int _pointer;
    private Transform _transform;
    private List<Node> _path;
    private List<Node> _nodes;
    private Node _closestNode;
    private Node _destinyNode;
    private Avoid _avoid;

    private AStar<Node> _aStar = new AStar<Node>();

    public SearchState(FSM<MinionController.States> fsm, MinionController minionController)
    {
        _fsm = fsm;
        _minionController = minionController;
        _transform = minionController.transform;
        _nodes = GameManager.Instance.nodesList;
        _minionsList = minionController.minionsList;

        if (minionController.isBoss == true)
        {
            _avoid = new Avoid(_minionController.transform, _minionController.lineOfSight.obstaclesLayer, _minionController.obstacleRadius, _minionController.obstacleWeight);
        }
    }

    public override void Awake()
    {
        //Debug.Log("Search State Awake");
        if (_minionController.isBoss == false) { return; }
        _closestNode = FindNearestNode();
        _destinyNode = GetRandomNode();
        _pointer = 0; // Reset Pointer.
        _avoid.SetTarget(_minionController.currentNode);
        _path = _aStar.Run(_closestNode, Satisfies, GetNeighbours, GetCost, Heuristic);
    }

    public override void Execute()
    {
        if (_pointer < _path.Count)
        {
            _avoid.SetTarget(_path[_pointer].transform);
            _minionController.Move(_avoid.GetDirection());
            _minionController.Look(_path[_pointer].transform.position);

            Vector3 target = _path[_pointer].transform.position;
            Vector3 difference = target - _transform.transform.position;
            var waypointDistace = difference.magnitude;

            if (waypointDistace <= _minionController.enemyMovement.safeDistanceToWaypoint) // Reached Waypoint
            {
                _pointer++;
            }
        }

        else
        {
            foreach (var minion in _minionsList)
            {
                minion.GetComponent<MinionController>()._fsm.Transition(MinionController.States.IDLE);
                _pointer = 0;
            }
        }
    }

    public override void Sleep()
    {
        //Debug.Log("Search State Sleep");
    }

    // ===== Search Methods ===== \\
    private Node FindNearestNode()
    {
        //Declaro variables
        float currentDistance = float.PositiveInfinity;
        Node nearestNode = null;

        //Recorro todos los nodos
        foreach (var item in _nodes)
        {
            //Calculo la distancia
            Vector3 diff = item.transform.position - _transform.transform.position;
            float dist = diff.magnitude;
            //Verifico si no hay obstaculos entre medio y si la distancia es menor a la distancia del nodo anterior
            bool isFree = Physics.Raycast(_transform.transform.position, diff.normalized, dist, _minionController.lineOfSight.obstaclesLayer);
            if (dist < currentDistance && !isFree)
            {
                currentDistance = dist;
                nearestNode = item;
            }
        }
        return nearestNode;
    }
    private Node GetRandomNode()
    {
        return _nodes[Random.Range(0, _nodes.Count - 1)];
    }
    private bool Satisfies(Node curr)
    {
        return curr == _destinyNode;
    }
    private List<Node> GetNeighbours(Node curr)
    {
        var list = new List<Node>();
        for (int i = 0; i < curr._neighbours.Count; i++)
        {
            list.Add(curr._neighbours[i]);
        }
        return list;
    }
    private float Heuristic(Node curr)
    {
        float cost = 0;
        // if (curr.hasTrap) cost += 5;
        cost += Vector3.Distance(curr.transform.position, _destinyNode.transform.position);
        return cost;
    }
    private float GetCost(Node p, Node c)
    {
        return Vector3.Distance(p.transform.position, c.transform.position);
    }
    public void FindPath()
    {
        _pointer = 0;
        _closestNode = FindNearestNode();
        _destinyNode = GetRandomNode();
        _minionController.currentNode = _destinyNode.transform;
        _path = _aStar.Run(_closestNode, Satisfies, GetNeighbours, GetCost, Heuristic);
    }
}
