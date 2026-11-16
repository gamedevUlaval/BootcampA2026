using System.Linq;
using UnityEngine;

public class CombatState : State
{
    private Health currentTarget;
    private float lastAttackTime;
    private float attackCooldown => 1 / unit.getAttackSpeed();

    public HealthDetector TargetDetector;

    public void SetTarget(Health newTarget) 
    {
        if(currentTarget != null)
            currentTarget.OnDeath -= OnTargetDeath;

        if (newTarget != null)
            newTarget.OnDeath += OnTargetDeath;

        currentTarget = newTarget;
    }

    public override void OnEnter()
    {
        if(currentTarget == null)
            SetTarget(GetNextTarget());

        TargetDetector.OnLost += OnUnitLost;
    }

    public override void OnExit()
    {
        unit.Agent.ResetPath();
        SetTarget(null);

        TargetDetector.OnLost -= OnUnitLost;
    }

    public override void UpdateState()
    {
        if (currentTarget == null)
        {
            SelectNextTarget();
            return;
        }

        if (DistanceFromTarget() > unit.getAttackRange())
        {
            unit.Agent.destination = currentTarget.transform.position;
        }
        else
        {
            unit.Agent.ResetPath();

            if (Time.time - lastAttackTime >= attackCooldown) 
            {
                currentTarget.CurrentHealthPoints -= unit.GetAttackDamageAgainst(currentTarget.Type);
                lastAttackTime = Time.time;
            }
        }
    }

    private float DistanceFromTarget() 
    {
        return Vector3.Distance(transform.position, currentTarget.transform.position);
    }

    private void OnTargetDeath()
    {
        SelectNextTarget();
    }

    private void SelectNextTarget()
    {
        Health next = GetNextTarget();

        if (next != null)
            SetTarget(next);
        else
            unit.DefendPosition();
    }

    private Health GetNextTarget()
    {
        return TargetDetector.DetectedList.FirstOrDefault(target => target != null && !target.IsDead);
    }

    private void OnUnitLost(Health lost)
    {
        if(currentTarget == lost)
            SelectNextTarget();
    }

    private void OnDestroy()
    {
        if (currentTarget != null)
            currentTarget.OnDeath -= OnTargetDeath;
    }
}