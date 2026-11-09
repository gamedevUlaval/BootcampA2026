using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StateMachine), typeof(Canvas))]
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private List<SceneUIContext> gameStateToUIStateMapper = new List<SceneUIContext>();

    private Dictionary<State, State> gameStateToUIStateMap = new Dictionary<State, State>();
    private StateMachine stateMachine;
    private Canvas canvas;

    [Serializable]
    private class SceneUIContext
    {
        public State gameState;
        public State UIState;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject.transform.root);
            stateMachine = GetComponent<StateMachine>();
            canvas = GetComponent<Canvas>();
            GameManager.I.onGameStateBranchChanged += OnStateChanged;
            foreach(SceneUIContext context in gameStateToUIStateMapper)
            {
                gameStateToUIStateMap.Add(context.gameState, context.UIState);
            }
            return;
        }
        Destroy(gameObject);
    }

    private void Update()
    {
       stateMachine.UpdateBranch();
    }

    private void OnStateChanged(List<State> stateBranch)
    {
        State UIState = null;
        for (int i = 0; i < stateBranch.Count; i++)
        {
            if (gameStateToUIStateMap.TryGetValue(stateBranch[i], out UIState))
            {
                stateMachine.SetState(UIState, forceReset: true, data: canvas);
                return;
            }
        }
    }
}
