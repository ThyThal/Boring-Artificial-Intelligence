using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _orangeBoss;
    [SerializeField] private GameObject _greenBoss;

    [SerializeField] private Text _greenStatus;
    [SerializeField] private Text _greenLife;

    [SerializeField] private Text _orangeStatus;
    [SerializeField] private Text _orangeLife;

    EnemyController _orangeController;
    EnemyController _greenController;

    private void Awake()
    {
        _orangeController = _orangeBoss.GetComponent<EnemyController>();
        _greenController = _greenBoss.GetComponent<EnemyController>();
    }


    private void Update()
    {
        if (_orangeBoss != null)
        {
            _orangeStatus.text = $"Status: {_orangeController.fsm.GetCurrentState()}";
            _orangeLife.text = $"Life: {_orangeController.lifeController.GetCurrentLife()}";
        }

        if (_greenBoss != null)
        {
            _greenStatus.text = $"Status: {_greenController.fsm.GetCurrentState()}";
            _greenLife.text = $"Life: {_greenController.lifeController.GetCurrentLife()}";
        }
    }
}

