using System;
using System.Linq;
using UnityEngine;

public class CombatState : State
{
    private AiUnit target;
    private float lastAttackTime;
    private float attackCooldown => 1 / unit.getAttackSpeed();

    //Change for a health detector
    public AiUnitDetector AiUnitDetector;

    public void SetTarget(AiUnit newTarget) 
    {
        if(target != null)
            target.OnDeath -= OnTargetDeath;

        if (newTarget != null)
            newTarget.OnDeath += OnTargetDeath;

        target = newTarget;
    }

    public override void OnEnter()
    {
        if(target == null)
            SetTarget(GetNextTarget());

        AiUnitDetector.OnUnitLost += OnUnitLost;
    }

    public override void OnExit()
    {
        unit.Agent.ResetPath();
        SetTarget(null);

        AiUnitDetector.OnUnitLost -= OnUnitLost;
    }

    public override void UpdateState()
    {
        if (target == null)
        {
            SelectNextTarget();
            return;
        }

        if (DistanceFromTarget() > unit.getAttackRange())
        {
            unit.Agent.destination = target.transform.position;
        }
        else
        {
            unit.Agent.ResetPath();

            if (Time.time - lastAttackTime >= attackCooldown) 
            {
                target.TakeDamage(unit.GetAttackDamageAgainst(target.UnitType));
                lastAttackTime = Time.time;
            }
        }
    }

    private float DistanceFromTarget() 
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    private void OnTargetDeath()
    {
        SelectNextTarget();
    }

    private void SelectNextTarget()
    {
        AiUnit next = GetNextTarget();

        if (next != null)
            SetTarget(next);
        else
            unit.DefendPosition();
    }

    private AiUnit GetNextTarget()
    {
        return AiUnitDetector.UnitsInDetectionRange.FirstOrDefault(unit => unit != null && !unit.IsDead);
    }

    private void OnUnitLost(AiUnit lostUnit)
    {
        if(target == lostUnit)
            SelectNextTarget();
    }
}
