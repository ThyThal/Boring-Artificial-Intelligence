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

    MinionController _orangeController;
    MinionController _greenController;

    private void Awake()
    {
        _orangeController = _orangeBoss.GetComponent<MinionController>();
        _greenController = _greenBoss.GetComponent<MinionController>();
    }


    private void Update()
    {
        if (_orangeBoss != null)
        {
            _orangeStatus.text = $"Status: {_orangeController._fsm.GetCurrentState()}";
            _orangeLife.text = $"Life: {_orangeController.lifeController.GetCurrentLife()}";
        }

        if (_greenBoss != null)
        {
            _greenStatus.text = $"Status: {_greenController._fsm.GetCurrentState()}";
            _greenLife.text = $"Life: {_greenController.lifeController.GetCurrentLife()}";
        }
    }
}

