using UnityEngine;

public class FlockState<T> : FSMState<T>
{
    private EnemyController _enemyController;

    public FlockState(EnemyController enemyController)
    {
        _enemyController = enemyController;
    } // Constructor del Estado.

    private void Awake()
    {

    }
     
    private void Execute()
    {

    }
}
