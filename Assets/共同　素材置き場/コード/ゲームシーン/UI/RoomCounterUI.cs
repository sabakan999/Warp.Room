using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCounterUI : MonoBehaviour
{
    [Header("テキスト")]
    public Text prefixText;   // 「あと」「現在」
    public Text numberText;   // 数字
    public Text suffixText;   // 「Room」「Room目」

    private int roomCount;

    // =========================
    // 初期化
    // =========================
    public void Init(int totalRooms)
    {
        if (!GameSettings.isEndlessMode)
        {
            // 通常モード
            roomCount = totalRooms;

            prefixText.text = "あと";
            suffixText.text = "Room";
        }
        else
        {
            // エンドレス
            roomCount = 0;

            prefixText.text = "現在";
            suffixText.text = "Room";
        }

        UpdateUI(false);
    }

    // =========================
    // 部屋進行
    // =========================
    public void DecreaseRoom()
    {
        if (!GameSettings.isEndlessMode)
        {
            // 通常：減る
            roomCount--;
        }
        else
        {
            // エンドレス：増える
            roomCount++;
        }

        UpdateUI(true);
    }

    // =========================
    // UI更新
    // =========================
    void UpdateUI(bool playAnimation)
    {
        numberText.text = roomCount.ToString();

        if (playAnimation)
        {
            numberText.transform.localScale = Vector3.zero;

            numberText.transform
                .DOScale(1.3f, 0.2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    numberText.transform.DOScale(1f, 0.1f);
                });
        }
    }
}