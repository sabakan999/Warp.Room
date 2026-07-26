using UnityEngine;
using UnityEngine.UI;

public class SettingBGM : SettingItem
{
    [Header("表示")]
    public Slider slider;

    [Header("変更量")]
    public float step = 0.1f;

    void Start()
    {
        slider.value = OptionSettings.BGMVolume;
    }

    public override void OnLeft()
    {
        SetVolume(slider.value - step);
    }

    public override void OnRight()
    {
        SetVolume(slider.value + step);
        Debug.Log("Left");
        
    }

    void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);

        slider.value = value;

        OptionSettings.BGMVolume = value;
    }
}