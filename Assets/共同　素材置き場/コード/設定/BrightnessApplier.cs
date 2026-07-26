using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessApplier : MonoBehaviour
{
    public Volume volume;

    void Start()
    {
        if (volume.profile.TryGet(out ColorAdjustments color))
        {
            color.postExposure.value = OptionSettings.Brightness;
        }
    }

    void Update()
    {
        if (volume.profile.TryGet(out ColorAdjustments color))
        {
            color.postExposure.value = OptionSettings.Brightness;
        }
    }


}