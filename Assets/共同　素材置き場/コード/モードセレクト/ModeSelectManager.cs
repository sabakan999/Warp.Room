using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class ModeSelectManager : MonoBehaviour
{
    [Header("選択肢UI")]
    public RectTransform normalButton;
    public RectTransform endlessButton;

    [Header("装飾フレーム")]
    public CanvasGroup normalFrame;
    public CanvasGroup endlessFrame;

    [Header("拡大設定")]
    public float selectedScale = 1.2f;
    public float normalScale = 1.0f;
    public float scaleTime = 0.15f;

    [Header("フレームフェード")]
    public float fadeTime = 0.25f;

    [Header("次シーン")]
    public string normalNextScene = "ステージセレクト";
    public string endlessNextScene = "ワープ・ルーム";

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;

    private int selectedIndex = 0; // 0=左 1=右
    private float prevH = 0f;
    private bool isDeciding = false; // 入力ロック

    void Start()
    {
        UpdateSelection(true);
        UpdateFrame(true);
    }

    void Update()
    {
        if (isDeciding) return;

        HandleMove();
        HandleSubmit();
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");

        int prevIndex = selectedIndex;

        if (h > 0.5f && prevH <= 0.5f)
        {
            selectedIndex++;
        }
        else if (h < -0.5f && prevH >= -0.5f)
        {
            selectedIndex--;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, 1);
        prevH = h;

        // 🔊 移動音（変わったときだけ）
        if (selectedIndex != prevIndex)
        {
            PlaySE(moveSE);
        }

        UpdateSelection(false);
        UpdateFrame(false);
    }

    // =========================
    // 🎯 ボタン拡大
    // =========================
    void UpdateSelection(bool instant)
    {
        if (normalButton == null || endlessButton == null)
            return;

        float nScale = (selectedIndex == 0) ? selectedScale : normalScale;
        float eScale = (selectedIndex == 1) ? selectedScale : normalScale;

        if (instant)
        {
            normalButton.localScale = Vector3.one * nScale;
            endlessButton.localScale = Vector3.one * eScale;
        }
        else
        {
            normalButton.DOKill();
            endlessButton.DOKill();

            normalButton
                .DOScale(nScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(normalButton.gameObject);

            endlessButton
                .DOScale(eScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(endlessButton.gameObject);
        }
    }

    // =========================
    // 🎨 フレーム
    // =========================
    void UpdateFrame(bool instant)
    {
        if (normalFrame == null || endlessFrame == null)
            return;

        float nAlpha = (selectedIndex == 0) ? 1f : 0f;
        float eAlpha = (selectedIndex == 1) ? 1f : 0f;

        if (instant)
        {
            normalFrame.alpha = nAlpha;
            endlessFrame.alpha = eAlpha;
        }
        else
        {
            normalFrame.DOKill();
            endlessFrame.DOKill();

            normalFrame
                .DOFade(nAlpha, fadeTime)
                .SetEase(Ease.OutQuad)
                .SetLink(normalFrame.gameObject);

            endlessFrame
                .DOFade(eAlpha, fadeTime)
                .SetEase(Ease.OutQuad)
                .SetLink(endlessFrame.gameObject);
        }
    }

    // =========================
    // 🎮 決定
    // =========================
    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectModeCoroutine());
        }
    }

    IEnumerator SelectModeCoroutine()
    {
        isDeciding = true;

        DOTween.KillAll();

        // 🔊 決定音
        PlaySE(decideSE);

        // 🔥 音が鳴り終わるまで待つ
        float waitTime = 0.2f;

        if (decideSE != null)
            waitTime = decideSE.length;

        yield return new WaitForSeconds(waitTime);

        if (selectedIndex == 0)
        {
            GameSettings.isEndlessMode = false;
            SceneManager.LoadScene(normalNextScene);
        }
        else
        {
            GameSettings.isEndlessMode = true;
            SceneManager.LoadScene(endlessNextScene);
        }
    }

    // =========================
    // 🔊 SE再生
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}