using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BlinkFade : MonoBehaviour
{
    [Header("点滅設定")]
    public float fadeTime = 0.8f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    private Graphic graphic;
    private Tween blinkTween; // 🔥 追加

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

        // 🔥 Tweenを保持 + 自動Kill
        blinkTween = graphic
            .DOFade(minAlpha, fadeTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject); // ← これが超重要
    }

    void OnDestroy()
    {
        // 🔥 念のため完全停止
        if (blinkTween != null && blinkTween.IsActive())
            blinkTween.Kill();
    }
}