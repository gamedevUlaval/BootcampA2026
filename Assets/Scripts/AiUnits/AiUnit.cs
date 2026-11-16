using System;
using UnityEngine;
using UnityEngine.AI;

public class AiUnit : MonoBehaviour
{
    [SerializeField] private AiUnitStats stats;
    [SerializeField] private UnitType type;
    [SerializeField] private Team team;

    private State currentState;
    private FollowState followState;
    private IdleState IdleState;
    private CombatState combatState;
    private State lastState;

    public Health Health;

    private HealthDetector targetDetector;

    public NavMeshAgent Agent;

    public State CurrentState
    {
        get { return currentState; }

        private set 
        {
            currentState?.OnExit();
            lastState = currentState;
            currentState = value;
            currentState?.OnEnter();
        }
    }

    public UnitType UnitType => type;
    public Team Team => team;
    public AiUnitStats Stats => stats;

    private void Awake()
    {
        Health = GetComponent<Health>();

        followState = GetComponentInChildren<FollowState>();
        IdleState = GetComponentInChildren<IdleState>();
        combatState = GetComponentInChildren<CombatState>();
        targetDetector = GetComponentInChildren<HealthDetector>();

        combatState.TargetDetector = targetDetector;

        Agent = GetComponent<NavMeshAgent>();

        Agent.speed = stats.MoveSpeed;
        Health.Initialize(stats.HealthPoints, type);
    }

    private void Start()
    {
        targetDetector.OnLost += OnTargetLost;
        targetDetector.OnDetected += OnTargetDetected;

        DefendPosition();
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void FollowTarget(Transform target)
    {
        followState.SetTarget(target);
        CurrentState = followState;
    }

    public void DefendPosition()
    {
        if (!HasTargetInRange())
        {
            IdleState.SetPosition(transform.position);
            CurrentState = IdleState;
        }
        else if (CurrentState != combatState)
        {
            EnterCombat(null);
        }
    }

    public void EnterCombat(Health target)
    {
        combatState.SetTarget(target);
        CurrentState = combatState;
    }

    public bool HasTargetInRange()
    {
        return targetDetector.DetectedList.Exists(target => target != null && !target.IsDead);
    }

    public float getAttackRange() { return stats.AttackRange; }
    public float getAttackSpeed() { return stats.AttackSpeed; }

    public int GetAttackDamageAgainst(UnitType defenderType)
    {
        float multiplier = 1f;

        if (type.Beats(defenderType))
            multiplier = stats.AdvantageMultiplier;
        else if (defenderType.Beats(type))
            multiplier = stats.DisadvantageMultiplier;

        return Mathf.Max(1, Mathf.RoundToInt(stats.AttackDamage * multiplier));
    }

    private void OnTargetLost(Health target)
    {
        if (!HasTargetInRange() && CurrentState == combatState)
        {
            if (lastState != null)
                CurrentState = lastState;
            else
                DefendPosition();
        }
    }

    private void OnTargetDetected(Health target)
    {
        if (CurrentState == IdleState)
        {
            EnterCombat(target);
        }
    }
}