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

    private AiUnitDetector aiUnitDetector;

    public Action OnDeath;

    public NavMeshAgent Agent;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => stats.HealthPoints;
    public bool IsDead { get; private set; }

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
        followState = GetComponentInChildren<FollowState>();
        IdleState = GetComponentInChildren<IdleState>();
        combatState = GetComponentInChildren<CombatState>();
        aiUnitDetector = GetComponentInChildren<AiUnitDetector>();

        combatState.AiUnitDetector = aiUnitDetector;

        Agent = GetComponent<NavMeshAgent>();

        Agent.speed = stats.MoveSpeed;
        CurrentHealth = stats.HealthPoints;
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
        if (!HasLivingEnemies())
        {
            IdleState.SetPosition(transform.position);
            CurrentState = IdleState;
        }
        else if (CurrentState != combatState)
        {
            EnterCombat(null);
        }
    }

    public void EnterCombat(AiUnit target)
    {
        combatState.SetTarget(target);
        CurrentState = combatState;
    }

    public bool HasLivingEnemies()
    {
        return aiUnitDetector.UnitsInDetectionRange.Exists(unit => unit != null && !unit.IsDead);
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

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        if (CurrentHealth == 0)
        {
            IsDead = true;
            Destroy(gameObject);
        }
    }

    private void OnUnitLost(AiUnit unit)
    {
        if (!HasLivingEnemies() && CurrentState == combatState)
        {
            if (lastState != null)
                CurrentState = lastState;
            else
                DefendPosition();
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
