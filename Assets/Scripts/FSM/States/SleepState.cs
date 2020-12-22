using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState<T> : FSMState<T>
{
    [SerializeField] private float _timer;
    [SerializeField] private EnemyController _enemyController;

    public SleepState(EnemyController enemyController)
    {
        _enemyController = enemyController;
    } // Constructor del Estado.
    public override void Awake()
    {
        Sleep();
    } // Sobreescribir Awake.
    public override void Execute()
    {
        if (_timer <= 0)
        {
            _enemyController.ExecuteTreeFromSleep();
        }
        _timer -= Time.deltaTime;
    }

    public override void Sleep()
    {
        _timer = Random.Range(3, 6);
    }
}
