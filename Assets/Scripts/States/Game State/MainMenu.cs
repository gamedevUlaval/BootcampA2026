using UnityEngine;

public class MainMenu : GameState<GameManager>
{
    public void Play()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(Play)]);
    }

    public void Settings()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(Settings)]);
    }
}
