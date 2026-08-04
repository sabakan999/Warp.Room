using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [Header("ステージ情報")]
    public int world;
    public int stage;


    [Header("ロック表示")]
    public GameObject lockImage;


    private Text text;
    private Vector3 defaultScale;


    void Start()
    {
        text = GetComponent<Text>();

        defaultScale = transform.localScale;

        
    }


    // =========================
    // 🔒 ロック更新
    // =========================
    public void UpdateLock()
    {
        bool unlocked = IsUnlocked();


        if (lockImage != null)
        {
            // 解放済み → 鍵OFF
            // 未解放 → 鍵ON
            lockImage.SetActive(!unlocked);
        }
    }


    // =========================
    // 🔓 解放判定
    // =========================
    public bool IsUnlocked()
{
    // 過去ワールド
    if(world < GameSettings.unlockedWorld)
    {
        return true;
    }


    // 未来ワールド
    if(world > GameSettings.unlockedWorld)
    {
        return false;
    }


    // 同じワールドならステージ比較
    return stage <= GameSettings.unlockedStage;
}


    // =========================
    // カーソル選択演出
    // =========================
    public void SetSelected(bool isSelected)
    {
        transform.localScale =
            isSelected ? defaultScale * 1.3f : defaultScale;
    }
}