using UnityEngine;

public class FollowState : State
{
    private Transform targetToFollow;

    public void SetTarget(Transform target) { targetToFollow = target; }

    public override void OnEnter(){}
    public override void OnExit()
    {
        unit.Agent.ResetPath();
    }

    public override void UpdateState()
    {
        unit.Agent.destination = targetToFollow.position;
    }
}