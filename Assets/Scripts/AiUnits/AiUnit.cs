using UnityEngine;
using UnityEngine.AI;

public class AiUnit : MonoBehaviour
{
    [SerializeField] private AiUnitStats stats;
    [SerializeField] private UnitType type;

    private State currentState;
    private FollowState followState;
    private DefendPositionState defendPositionState;
    private CombatState combatState;

    public NavMeshAgent Agent;

    public State CurrentState
    {
        get { return currentState; }

        private set 
        {
            currentState?.OnExit();
            currentState = value;
            currentState.OnEnter();
        }
    }

    public UnitType UnitType => type;

    private void Awake()
    {
        followState = GetComponentInChildren<FollowState>();
        defendPositionState = GetComponentInChildren<DefendPositionState>();
        combatState = GetComponentInChildren<CombatState>();

        Agent = GetComponent<NavMeshAgent>();

        Agent.speed = stats.MoveSpeed;
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
        defendPositionState.SetPosition(transform.position);
        CurrentState = defendPositionState;
    }

    public void EnterCombat(AiUnit target)
    {
        combatState.SetTarget(target);
        CurrentState = combatState;
    }

    public float getAttackRange() { return stats.AttackRange; }
    public float getAttackSpeed() { return stats.AttackSpeed; }
}