using UnityEngine;

public class PauseUI : UIState<Pause>
{
    public void OnResumeBtnClick()
    {
        representedObject.Play();
    }

    public void OnMainMenuBtnClick()
    {
        representedObject.MainMenu();
    }

    public void OnSettingsBtnClick()
    {
        representedObject.Settings();
    }
}
