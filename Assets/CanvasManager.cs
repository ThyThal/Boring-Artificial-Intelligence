using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _orangeBoss;
    [SerializeField] private GameObject _greenBoss;

    [SerializeField] private Text _orangeText;
    [SerializeField] private Text _greenText;

    EnemyController _orangeController;
    EnemyController _greenController;

    private void Awake()
    {
        _orangeController = _orangeBoss.GetComponent<EnemyController>();
        _greenController = _greenBoss.GetComponent<EnemyController>();
    }


    private void Update()
    {
        _greenText.text = $"Status: {_greenController.fsm.GetCurrentState()}";
        _orangeText.text = $"Status: {_orangeController.fsm.GetCurrentState()}";
    }
}

