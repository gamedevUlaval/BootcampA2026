using UnityEngine;

public class CombatState : State
{
    private AiUnit target;

    private float lastAttackTime;

    private float attackCooldown => 1 / unit.getAttackSpeed();

    public void SetTarget(AiUnit target) { this.target = target; }

    public override void OnEnter(){}
    public override void OnExit()
    {
        unit.Agent.ResetPath();
    }

    public override void UpdateState()
    {
        if (DistanceFromTarget() > unit.getAttackRange())
        {
            unit.Agent.destination = target.transform.position;
        }
        else
        {
            unit.Agent.ResetPath();

            if (Time.time - lastAttackTime >= attackCooldown) 
            {
                Debug.Log("Attack");
                lastAttackTime = Time.time;
            }
        }
    }

    private float DistanceFromTarget() 
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }
}