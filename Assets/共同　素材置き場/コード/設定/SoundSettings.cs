using UnityEngine;

public static class SoundSettings
{
    public static float BGMVolume
    {
        get => PlayerPrefs.GetFloat("BGMVolume", 1f);
        set
        {
            PlayerPrefs.SetFloat("BGMVolume", value);
            PlayerPrefs.Save();
        }
    }

    public static float SEVolume
    {
        get => PlayerPrefs.GetFloat("SEVolume", 1f);
        set
        {
            PlayerPrefs.SetFloat("SEVolume", value);
            PlayerPrefs.Save();
        }
    }
}