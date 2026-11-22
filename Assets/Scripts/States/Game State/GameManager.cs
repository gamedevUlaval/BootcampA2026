using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    public Action<List<State>> onGameStateBranchChanged;

    [SerializeField] private StateMachine gameStateMachine;

    //Use only for ingame cutscene
    private bool isPaused = false;

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
        }
        else
        {
            I = this;
            DontDestroyOnLoad(gameObject.transform.root);
            SubscribeToBranchChanges();
        }
    }

    public void CutscenePause() 
    {
        isPaused = true;
    }

    public void CutsceneUnpause() 
    {
        isPaused = false;
    }

    private void Update()
    {
        if (!isPaused)
        {
            gameStateMachine.UpdateBranch();
        }
    }

    private void Start() 
    {
        gameStateMachine.Initialize();
    }

    public T GetStateInBranch<T>() where T : State
    {
        State state = gameStateMachine.GetCurrentState();
        while(state != null && state.GetType() != typeof(T))
        {
            state = state.GetChildState();
        }
        return state as T;
    }

    private void SubscribeToBranchChanges()
    {
        StateMachine[] stateMachines = GetComponentsInChildren<StateMachine>();
        foreach(StateMachine stateMachine in stateMachines)
        {
            stateMachine.onStateChange += OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(List<State> stateBranch)
    {
        onGameStateBranchChanged?.Invoke(stateBranch);
    }
}
