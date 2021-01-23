﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSearchState<T> : FSMState<T>
{
    [SerializeField] private List<Node> _nodes;
    [SerializeField] private List<Node> _path;
    [SerializeField] private Transform _transform;
    [SerializeField] private LayerMask _obstacles;
    [SerializeField] private Node _closestNode;
    [SerializeField] private Node _destiny;
    [SerializeField] private int _pointer;
    [SerializeField] private EnemyController _myEnemyController;
    [SerializeField] private Avoid _avoid;

    AStar<Node> _theta = new AStar<Node>();

    public OldSearchState(List<Node> nodes, Transform transform, LayerMask obstacles, EnemyController myEnemyController, Transform targetTransform)
    {
        _nodes = nodes;
        _transform = transform;
        _obstacles = obstacles;
        _myEnemyController = myEnemyController;
        _avoid = new Avoid(_myEnemyController.transform, _obstacles, _myEnemyController.obstacleRadius, _myEnemyController.obstacleWeight);
    }

    public override void Awake()
    {
        //if (_path.Count > 0) _path.Clear();
        _pointer = 0;
        _closestNode = FindNearestNode();
        _destiny = GetRandomNode();
        _myEnemyController.currentNode = _destiny.transform;
        _avoid.SetTarget(_myEnemyController.currentNode);
        _path = _theta.Run(_closestNode, Satisfies, GetNeighbours, GetCost, Heuristic);
    }

    public override void Execute()
    {
        if (_pointer < _path.Count)
        {
            _avoid.SetTarget(_path[_pointer].transform);
            _myEnemyController.Move(_avoid.GetDirection());
            _myEnemyController.Look(_path[_pointer].transform.position);

            Vector3 target = _path[_pointer].transform.position;
            Vector3 difference = target - _transform.transform.position;
            var waypointDistace = difference.magnitude;

            if (waypointDistace <= _myEnemyController.enemyMovement.safeDistanceToWaypoint) // Reached Waypoint
            {
                _pointer++;
            }
        }

        else
        {
            _myEnemyController.ExecuteTreeFromSleep();
        }
    }

    Node FindNearestNode()
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
            bool isFree = Physics.Raycast(_transform.transform.position, diff.normalized, dist, _obstacles);
            if (dist < currentDistance && !isFree)
            {
                currentDistance = dist;
                nearestNode = item;
            }
        }
        return nearestNode;
    }
    Node GetRandomNode()
    {
        return _nodes[Random.Range(0, _nodes.Count - 1)];
    }
    bool Satisfies(Node curr)
    {
        return curr == _destiny;
    }
    List<Node> GetNeighbours(Node curr)
    {
        var list = new List<Node>();
        for (int i = 0; i < curr._neighbours.Count; i++)
        {
            list.Add(curr._neighbours[i]);
        }
        return list;
    }
    float Heuristic(Node curr)
    {
        float cost = 0;
        // if (curr.hasTrap) cost += 5;
        cost += Vector3.Distance(curr.transform.position, _destiny.transform.position);
        return cost;
    }
    float GetCost(Node p, Node c)
    {
        return Vector3.Distance(p.transform.position, c.transform.position);
    }
    public void FindPath()
    {
        _pointer = 0;
        _closestNode = FindNearestNode();
        _destiny = GetRandomNode();
        _myEnemyController.currentNode = _destiny.transform;
        _path = _theta.Run(_closestNode, Satisfies, GetNeighbours, GetCost, Heuristic);
    }
}