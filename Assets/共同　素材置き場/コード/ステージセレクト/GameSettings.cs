using UnityEngine;
public static class GameSettings
{
    public static int selectedWorld = 1;
    public static int selectedStage = 1;

    // 🔥 モード
    public static bool isEndlessMode = false;

    public static string playerName = "";
   public static int unlockedWorld = 1;
    public static int unlockedStage = 1;
    
    public static void Load()
    {
        unlockedWorld = PlayerPrefs.GetInt(
            "UnlockedWorld",
            1
        );

        unlockedStage = PlayerPrefs.GetInt(
            "UnlockedStage",
            1
        );
    }



   public static void UnlockNextStage()
{
    int nextWorld = selectedWorld;
    int nextStage = selectedStage + 1;

    if (selectedStage >= 3)
    {
        nextWorld = selectedWorld + 1;
        nextStage = 1;
    }

    // 緊張修正箇所
    if (nextWorld > unlockedWorld ||
        (nextWorld == unlockedWorld && nextStage > unlockedStage))
    {
        unlockedWorld = nextWorld;
        unlockedStage = nextStage;

        PlayerPrefs.SetInt("UnlockedWorld", unlockedWorld);
        PlayerPrefs.SetInt("UnlockedStage", unlockedStage);
        PlayerPrefs.Save();
    }
}
    
}