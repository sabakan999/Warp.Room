using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BlinkArrow : MonoBehaviour
{
    [Header("点滅")]
    public float fadeTime = 0.8f;
    [Range(0f, 1f)]
    public float minAlpha = 0.3f;
    [Range(0f, 1f)]
    public float maxAlpha = 1f;

    [Header("ぴょこん")]
    public float moveDistance = 12f;
    public float moveTime = 0.5f;

    private Image image;
    private RectTransform rect;
    private Vector2 startPos;

    void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        startPos = rect.anchoredPosition;
    }

    void OnEnable()
    {
        // 念のため前回のTweenを消す
        image.DOKill();
        rect.DOKill();

        // 点滅
        image.DOFade(minAlpha, fadeTime)
            .From(maxAlpha)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);

        // ぴょこん
        rect.DOAnchorPosY(startPos.y - moveDistance, moveTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    void OnDisable()
    {
        image.DOKill();
        rect.DOKill();

        rect.anchoredPosition = startPos;
    }
}