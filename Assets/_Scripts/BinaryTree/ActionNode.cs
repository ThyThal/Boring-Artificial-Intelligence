using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : INode
{
    public delegate void ActionDelegate();
    ActionDelegate _action;

    public ActionNode(ActionDelegate action)
    {
        _action = action;
    }
    public void SubAction(ActionDelegate newAction)
    {
        _action += newAction;
    }
    public void Execute()
    {
        _action();
    }
}
