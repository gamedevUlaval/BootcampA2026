using UnityEngine;

public class IdleState : State
{
    private Vector3 idleLocation;

    public void SetPosition(Vector3 position)
    {
        idleLocation = position;
    }

    public override void OnEnter()
    {
        idleLocation = transform.position;
    }

    public override void OnExit(){}

    public override void UpdateState()
    {

    }
}
