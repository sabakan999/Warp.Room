using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TutorialEndingManager : MonoBehaviour
{
    [Header("幕")]
    [SerializeField] private RectTransform curtain;

    [Header("設定")]
    [SerializeField] private float closeTime = 1.5f;
    [SerializeField] private float waitTime = 0.5f;

    [Header("遷移先")]
    [SerializeField] private string nextScene = "ModeSelect";

    private Vector2 startPos;
    private Vector2 endPos;

    void Awake()
    {
        startPos = curtain.anchoredPosition;

        // 幕が画面を覆う位置
        endPos = new Vector2(startPos.x, 0);
    }

    public void PlayEnding()
    {
        curtain.gameObject.SetActive(true);

        curtain.anchoredPosition = startPos;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            curtain.DOAnchorPos(endPos, closeTime)
                .SetEase(Ease.InQuad)
        );

        seq.AppendInterval(waitTime);

        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(nextScene);
        });
    }
}