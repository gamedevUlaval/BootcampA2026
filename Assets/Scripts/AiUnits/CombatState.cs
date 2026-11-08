using System;
using System.Linq;
using UnityEngine;

public class CombatState : State
{
    // change AiUnit for Hittable
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

    private void OnTargetDeath()
    {
        SetTarget(GetNextTarget());
    }

    private AiUnit GetNextTarget()
    {
        return AiUnitDetector.UnitsInDetectionRange.First();
    }
    private void OnUnitLost(AiUnit lostUnit)
    {
        if(target == lostUnit)
            SetTarget(GetNextTarget());
    }
}