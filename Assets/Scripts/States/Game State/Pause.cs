using UnityEngine;

public class Pause : GameState<GameManager>
{
    public void Play()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(Play)]);
    }

    public void MainMenu()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(MainMenu)]);
    }

    public void Settings()
    {
        _parentStateMachine.SetState(_parentStateMachine.states[typeof(Settings)]);
    }
}
