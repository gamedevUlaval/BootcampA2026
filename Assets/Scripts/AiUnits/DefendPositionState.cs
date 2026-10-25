using UnityEngine;

public class DefendPositionState : State
{
    private Vector3 defendingPosition;

    public void SetPosition(Vector3 position)
    {
        defendingPosition = position;
    }

    public override void OnEnter()
    {
        defendingPosition = transform.position;
    }

    public override void OnExit(){}

    public override void UpdateState()
    {

    }
}
