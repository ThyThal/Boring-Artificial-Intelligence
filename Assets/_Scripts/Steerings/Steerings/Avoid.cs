using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoid : ISteeringBehaviour
{
    [SerializeField] private Transform _myTransform;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _targetVector;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _obstacleRadius;
    [SerializeField] private float _obstacleWeight;

    public Avoid(Transform myTransform, LayerMask obstacleLayer, float obstacleRadius, float obstacleWeight)
    {
        _myTransform = myTransform;
        _obstacleLayer = obstacleLayer;
        _obstacleRadius = obstacleRadius;
        _obstacleWeight = obstacleWeight;
    }

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }

    public void SetTargetByVector(Vector3 target)
    {
        _targetVector = target;
    }

    public Vector3 GetDirection()
    {
        // Obtener todos los obstaculos en una esfera.
        Collider[] obstacles = Physics.OverlapSphere(_myTransform.position, _obstacleRadius, _obstacleLayer);
        Transform savedObstacle = null;
        var count = obstacles.Length;

        // Recorrer lista de obstaculos.
        for (int i = 0; i < count; i++)
        {
            var currentObstacle = obstacles[i].transform;

            // Si es nulo asignar valor.
            if (savedObstacle == null) 
            {
                savedObstacle = currentObstacle;
            } 

            // Si no es nulo comprobar distancia.
            else if (Vector3.Distance(_myTransform.position, savedObstacle.position) > Vector3.Distance(_myTransform.position, currentObstacle.position))
            {
                savedObstacle = currentObstacle;
            }
        }

        Vector3 target;

        if (_targetTransform == null)
        {
            target = _targetVector;
        }

        else
        {
            target = _targetTransform.position;
        }

        Vector3 directionToTarget = (target - _myTransform.position).normalized;

        if (savedObstacle != null)
        {
            Vector3 directionObstacleToNPC = (_myTransform.position - savedObstacle.position).normalized * _obstacleWeight;
            directionToTarget += directionObstacleToNPC;
        }

        return directionToTarget.normalized;
    }

}
