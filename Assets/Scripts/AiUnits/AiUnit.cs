using System;
using UnityEngine;
using UnityEngine.AI;

public class AiUnit : MonoBehaviour
{
    [SerializeField] private AiUnitStats stats;
    [SerializeField] private UnitType type;

    private State currentState;
    private FollowState followState;
    private IdleState IdleState;
    private CombatState combatState;
    private State lastState;

    //Change for a health detector
    private AiUnitDetector aiUnitDetector;

    //temporary until health/damage system
    public Action OnDeath;

    public NavMeshAgent Agent;

    public State CurrentState
    {
        get { return currentState; }

        private set 
        {
            currentState?.OnExit();
            lastState = currentState;
            currentState = value;
            currentState.OnEnter();
        }
    }

    public UnitType UnitType => type;

    private void Awake()
    {
        followState = GetComponentInChildren<FollowState>();
        IdleState = GetComponentInChildren<IdleState>();
        combatState = GetComponentInChildren<CombatState>();
        aiUnitDetector = GetComponentInChildren<AiUnitDetector>();

        combatState.AiUnitDetector = aiUnitDetector;

        Agent = GetComponent<NavMeshAgent>();

        Agent.speed = stats.MoveSpeed;
    }

    private void Start()
    {
        aiUnitDetector.OnUnitLost += OnUnitLost;
        aiUnitDetector.OnUnitDetected += OnUnitDetected;

        DefendPosition();
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    //temporary until health/damage system
    private void OnDestroy()
    {
        OnDeath?.Invoke();
    }

    public void FollowTarget(Transform target)
    {
        followState.SetTarget(target);
        CurrentState = followState;
    }

    public void DefendPosition()
    {
        if (aiUnitDetector.UnitsInDetectionRange.Count == 0)
        {
            IdleState.SetPosition(transform.position);
            CurrentState = IdleState;
        }
        else
        {
            EnterCombat(null);
        }
    }

    public void EnterCombat(AiUnit target)
    {
        combatState.SetTarget(target);
        CurrentState = combatState;
    }

    public float getAttackRange() { return stats.AttackRange; }
    public float getAttackSpeed() { return stats.AttackSpeed; }

    private void OnUnitLost(AiUnit unit)
    {
        if (aiUnitDetector.UnitsInDetectionRange.Count == 0 && CurrentState == combatState)
        {
            CurrentState = lastState;
        }
    }

    private void OnUnitDetected(AiUnit unit)
    {
        if (CurrentState == IdleState)
        {
            EnterCombat(unit);
        }
    }
}