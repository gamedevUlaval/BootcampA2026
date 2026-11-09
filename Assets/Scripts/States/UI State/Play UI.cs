using UnityEngine;

public class PlayUI : UIState<Play>
{
    public void OnPauseBtnClick()
    {
        representedObject.Pause();
    }
}
