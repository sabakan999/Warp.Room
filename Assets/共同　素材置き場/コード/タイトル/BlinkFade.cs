using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BlinkFade : MonoBehaviour
{
    [Header("点滅設定")]
    public float fadeTime = 0.8f;     // フェード時間
    public float minAlpha = 0.2f;     // 最低透明度
    public float maxAlpha = 1f;       // 最大透明度

    private Graphic graphic; // Text / Image 両対応

    void Start()
    {
        graphic = GetComponent<Graphic>();

        if (graphic == null)
        {
            Debug.LogWarning("Graphicが見つからない！");
            return;
        }

        StartBlink();
    }

    void StartBlink()
    {
        Color c = graphic.color;
        c.a = maxAlpha;
        graphic.color = c;

        // 🔁 無限フェード
        graphic
            .DOFade(minAlpha, fadeTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}