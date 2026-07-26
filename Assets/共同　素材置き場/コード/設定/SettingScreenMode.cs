using UnityEngine;
using UnityEngine.UI;

public class SettingScreenMode : SettingItem
{
    public Text valueText;

    public override void OnLeft()
    {
        ToggleMode();
    }

    public override void OnRight()
    {
        ToggleMode();
    }

    void Start()
    {
        UpdateText();
    }

    void ToggleMode()
    {
        OptionSettings.FullScreen = !OptionSettings.FullScreen;

        Screen.fullScreen = OptionSettings.FullScreen;

        UpdateText();
    }

    void UpdateText()
    {
        if (valueText != null)
        {
            valueText.text = OptionSettings.FullScreen
                ? "フルスクリーン"
                : "ウィンドウ";
        }
    }
}