using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class CurtainTransition : MonoBehaviour
{
    [Header("3本ライン")]
    public RectTransform topBar;
    public RectTransform middleBar;
    public RectTransform bottomBar;

    [Header("設定")]
    public float moveTime = 0.4f;
    public float delayBetween = 0.1f;
    public float offscreenX = 1000f; // 画面外距離

    bool isPlaying = false;

    void Start()
    {
        // 初期位置（左外）
        SetOffscreen();
    }

    void SetOffscreen()
    {
        topBar.anchoredPosition = new Vector2(-offscreenX, topBar.anchoredPosition.y);
        middleBar.anchoredPosition = new Vector2(-offscreenX, middleBar.anchoredPosition.y);
        bottomBar.anchoredPosition = new Vector2(-offscreenX, bottomBar.anchoredPosition.y);
    }

    public void Play(string nextScene)
    {
        if (isPlaying) return;
        StartCoroutine(Transition(nextScene));
    }

    IEnumerator Transition(string nextScene)
    {
        isPlaying = true;

        // =========================
        // ▶ 閉じる（上→中→下）
        // =========================
        yield return SlideIn();

        // シーン切り替え
        SceneManager.LoadScene(nextScene);

        // 1フレーム待つ（超重要）
        yield return null;

        // =========================
        // ▶ 開く（下→中→上）
        // =========================
        yield return SlideOut();

        isPlaying = false;
    }

    IEnumerator SlideIn()
    {
        yield return topBar.DOAnchorPosX(0, moveTime).SetEase(Ease.OutCubic).WaitForCompletion();

        yield return new WaitForSeconds(delayBetween);

        yield return middleBar.DOAnchorPosX(0, moveTime).SetEase(Ease.OutCubic).WaitForCompletion();

        yield return new WaitForSeconds(delayBetween);

        yield return bottomBar.DOAnchorPosX(0, moveTime).SetEase(Ease.OutCubic).WaitForCompletion();
    }

    IEnumerator SlideOut()
    {
        yield return bottomBar.DOAnchorPosX(offscreenX, moveTime).SetEase(Ease.InCubic).WaitForCompletion();

        yield return new WaitForSeconds(delayBetween);

        yield return middleBar.DOAnchorPosX(offscreenX, moveTime).SetEase(Ease.InCubic).WaitForCompletion();

        yield return new WaitForSeconds(delayBetween);

        yield return topBar.DOAnchorPosX(offscreenX, moveTime).SetEase(Ease.InCubic).WaitForCompletion();
    }
}