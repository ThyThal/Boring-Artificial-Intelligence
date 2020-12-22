using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMState<T>
{
    // Diccionario los estados del FSM.
    Dictionary<T, FSMState<T>> _states = new Dictionary<T, FSMState<T>>();

    // Funciones que ejecuta la FSM.
    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    // Metodos de los estados.
    public void AddTransition(T input, FSMState<T> state)
    {
        if (!_states.ContainsKey(input))
        {
            _states.Add(input, state);
        }
    } // Crea una transicion a un estado.
    public void RemoveTransition(T input)
    {
        if (_states.ContainsKey(input))
        {
            _states.Remove(input);
        }
    } // Elimina una transicion de un estado.
    public FSMState<T> GetTransition(T input)
    {
        if (_states.ContainsKey(input))
        {
            return _states[input];
        }

        else
        {
            return null;
        }
    } // Obtiene el estado de un input.
}
