using UnityEngine;

public class SettingsUI : UIState<Settings>
{
    public void OnReturnBtnClick()
    {
        representedObject.Return();
    }
}
