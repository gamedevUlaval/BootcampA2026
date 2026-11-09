using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public Action<List<State>> onStateChange; // Returns the branch. Index 0 is the state that was changed.
    public Dictionary<Type, State> states = new Dictionary<Type, State>();

    [SerializeField] private State initialState;

    private State _lastState;
    private State _state;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out State state))
            {
                AddState(state);
            }
        }
    }

    public void Initialize()
    {
        if (initialState)
        {
            SetState(initialState);
        }
    }

    public State SetState(State nextState, bool forceReset = false, object data = null)
    {
        _lastState = _state;
        if(_state != nextState || (_state == nextState && forceReset))
        {
            _state?.StateExit(data, nextState?.GetType());
        }
        _state = nextState;
        _state?.StateEnter(data, _lastState?.GetType());
        onStateChange?.Invoke(GetBranch());
        return _lastState;
    }

    public void AddState(State newState)
    {
        states.Add(newState.GetType(), newState);
        newState.SetMachine(this);
    }

    public List<State> GetBranch()
    {
        List<State> stateBranch = new List<State>() { _state };
        while (stateBranch[stateBranch.Count - 1].GetChildState() != null) 
        {
            stateBranch.Add(stateBranch[stateBranch.Count - 1].GetChildState());
        }
        return stateBranch;
    }

    public State GetCurrentState()
    {
        return _state;
    }

    public void UpdateBranch()
    {
        _state?.StateUpdate();
    }

    public State GetInitialState()
    {
        return initialState;
    }

    public void ReturnToLastState()
    {
        SetState(_lastState);
    }
}
