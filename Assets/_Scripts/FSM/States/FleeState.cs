using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState<T> : FSMState<T>
{
    //Variables
    private Flee _flee;
    private EnemyController _myController;
    private Transform _enemyTransform;
    private Rigidbody _rigidbody;

    //Constructor
    public FleeState(EnemyController myController, Transform enemyTransform, Rigidbody rigidbody)
    {
        _myController = myController;
        _enemyTransform = enemyTransform;
        _rigidbody = rigidbody;
        _flee = new Flee(_myController.transform, _enemyTransform, _rigidbody, 0.5f);
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {

    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        _myController.Move(_flee.GetDirection());
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {

    }
}
