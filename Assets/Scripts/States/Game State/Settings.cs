using System;
using UnityEngine;

public class Settings : GameState<GameManager>
{
    private Type lastState;

    public override void StateEnter(object data, Type lastState)
    {
        base.StateEnter(data, lastState);
        this.lastState = lastState;
    }

    public void Return()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[lastState]);
    }
}
