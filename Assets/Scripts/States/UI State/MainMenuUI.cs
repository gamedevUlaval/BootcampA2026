using UnityEngine;

public class MainMenuUI : UIState<MainMenu>
{
    public void OnPlayBtnClick()
    {
        representedObject.Play();
    }

    public void OnSettingsBtnClick()
    {
        representedObject.Settings();
    }
}
