using System;
using UnityEngine;

public class Play : GameState<GameManager>
{
    public override void StateEnter(object data, Type lastState)
    {
        base.StateEnter(data, lastState);
    }

    public override void StateExit(object data, Type nextState)
    {
        base.StateExit(data, nextState);
    }

    public void Pause()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(Pause)]);
    }
}
