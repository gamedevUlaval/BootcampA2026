using UnityEngine;

public class FollowState : State
{
    [SerializeField] float stopDistance = 3f;

    private Transform targetToFollow;

    public void SetTarget(Transform target) { targetToFollow = target; }

    public override void OnEnter(){}
    public override void OnExit()
    {
        unit.Agent.ResetPath();
    }

    public override void UpdateState()
    {
        if(Vector3.Distance(transform.position, targetToFollow.position) > stopDistance)
            unit.Agent.destination = targetToFollow.position;
        else
            unit.Agent.destination = transform.position;
    }
}