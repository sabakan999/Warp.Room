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
        // 次のステージ
        if(selectedStage < 3)
        {
            unlockedWorld = selectedWorld;
            unlockedStage = selectedStage + 1;
        }
        // 3クリア → 次ワールド
        else
        {
            unlockedWorld = selectedWorld + 1;
            unlockedStage = 1;
        }


        PlayerPrefs.SetInt(
            "UnlockedWorld",
            unlockedWorld
        );

        PlayerPrefs.SetInt(
            "UnlockedStage",
            unlockedStage
        );

        PlayerPrefs.Save();
    }
    
}