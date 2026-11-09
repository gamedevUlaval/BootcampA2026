using System;
using UnityEngine;

public class GameState<Parent> : State
{
    [SerializeField] protected Parent parent;

    public override void StateEnter(object data, Type lastState)
    {
        base.StateEnter(data, lastState);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit(object data, Type nextState)
    {
        base.StateExit(data, nextState);
    }
}
