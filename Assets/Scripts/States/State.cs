using System;
using UnityEngine;

public class State : MonoBehaviour
{
    [SerializeField] protected StateMachine childStateMachine;

    protected StateMachine _parentStateMachine;

    public virtual void SetMachine(StateMachine stateMachine)
    {
        _parentStateMachine = stateMachine;
    }

    public virtual void StateEnter(object data, Type lastState)
    {
        gameObject.SetActive(true);
    }

    public virtual void StateUpdate()
    {
        childStateMachine?.UpdateBranch();
    }

    public virtual void StateExit(object data, Type nextState)
    {
        gameObject.SetActive(false);
    }

    public State GetChildState()
    {
        return childStateMachine?.GetCurrentState();
    }

    public StateMachine GetParentStateMachine()
    {
        return _parentStateMachine;
    }
}
