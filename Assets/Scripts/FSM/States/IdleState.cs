using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    [SerializeField] private float _timer;
    [SerializeField] private EnemyController _enemyController;

    public IdleState(EnemyController enemyController)
    {
        _enemyController = enemyController;
    } // Constructor del Estado.

    public override void Awake()
    {
        _timer = Random.Range(1, 3);
    } // Sobreescribir Awake.

    public override void Execute()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }

        else
        {
            _enemyController.ExecuteBinaryTree();
        }
    }
}
