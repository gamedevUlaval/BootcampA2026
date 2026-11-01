using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected AiUnit unit;

    protected virtual void Awake()
    {
        unit = GetComponentInParent<AiUnit>();
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void UpdateState();
}