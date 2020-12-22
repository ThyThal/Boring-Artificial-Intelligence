using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    FSMState<T> _currentState; // Estado actual.

    public void SetInitState(FSMState<T> initState)
    {
        _currentState = initState;
        _currentState.Awake();
    } // Inicializar estado.
    public void OnUpdate()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    } // Update del estado actual.
    public void Transition(T input)
    {
        FSMState<T> newState = _currentState.GetTransition(input);

        if (newState != null)
        {
            _currentState.Sleep();
            _currentState = newState;
            _currentState.Awake();
        }
    } // Añadir transicion de nuevo estado.
    public FSMState<T> GetCurrentState()
    {
        return _currentState;
    } // Obtener estado actual.
}
