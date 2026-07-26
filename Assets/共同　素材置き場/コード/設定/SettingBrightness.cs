using UnityEngine;
using UnityEngine.UI;

public class SettingBrightness : SettingItem
{
    [Header("表示")]
    public Slider slider;

    [Header("変更量")]
    public float step = 0.1f;

    void Start()
    {
        slider.value = Mathf.InverseLerp(-2f, 2f, OptionSettings.Brightness);
    }

    public override void OnLeft()
    {
        SetBrightness(slider.value - step);
    }

    public override void OnRight()
    {
        SetBrightness(slider.value + step);
    }

    void SetBrightness(float value)
    {
        value = Mathf.Clamp01(value);

        slider.value = value;

        // Exposure(-2～2)に変換
        OptionSettings.Brightness = Mathf.Lerp(-2f, 2f, value);
    }
}