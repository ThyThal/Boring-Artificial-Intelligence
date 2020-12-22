using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState<T> : FSMState<T>
{
    //Variables
    private Flee _flee;
    private EnemyController _enemyController;
    private Transform _target;
    private Rigidbody _rigidbody;

    //Constructor
    public FleeState(EnemyController enemyController, Transform targetTransform, Rigidbody rigidbody)
    {
        _enemyController = enemyController;
        _target = targetTransform;
        _rigidbody = rigidbody;
        _flee = new Flee(_enemyController.transform, _target, _rigidbody, 0.5f);
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {

    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        _enemyController.Move(_flee.GetDirection());
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {

    }
}
