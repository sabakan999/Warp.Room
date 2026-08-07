using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleInput : MonoBehaviour
{
    [Header("遷移先シーン")]
    public string nextScene = "モードセレクト";

    [Header("UIまとめ")]
    public GameObject titleRoot;

    [Header("暗転")]
    public GameObject fadePanel;
    public float fadeTime = 0.5f;
    public float firstDelay = 1f;

    [Header("BGM")]
    public AudioSource bgmSource;

    [Header("SE（決定音）")]
    public AudioSource seSource;
    public AudioClip decideSE;

    public TitleTeleportPlayer teleportPlayer;

    // 🔥 ロゴ停止用
    public TitleLogoAnimator logoAnimator;

    private bool canInput = false;
    private bool isTransitioning = false;

    void Start()
    {
        if (titleRoot != null)
            titleRoot.SetActive(false);

        if (fadePanel != null)
            fadePanel.SetActive(false);

        if (bgmSource != null)
            bgmSource.Stop();

        StartCoroutine(TitleIntro());
    }

    IEnumerator TitleIntro()
    {
        yield return new WaitForSeconds(firstDelay);

        if (fadePanel != null)
            fadePanel.SetActive(true);

        yield return new WaitForSeconds(fadeTime);

        if (titleRoot != null)
            titleRoot.SetActive(true);

        if (teleportPlayer != null)
            teleportPlayer.StartTeleport();

        if (bgmSource != null)
            bgmSource.Play();

        if (fadePanel != null)
            fadePanel.SetActive(false);

        canInput = true;
    }

   void Update()
{
    if (!canInput || isTransitioning)
        return;

    // Aボタン（決定）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        isTransitioning = true;
        StartCoroutine(GoNextCoroutine());
        return;
    }

    // Bボタン（キャンセル → タイトルは戻る場所がないので同じ動作）
    if (Input.GetKeyDown(KeyCode.Backspace) ||
        Input.GetButtonDown("Cancel"))
    {
        isTransitioning = true;
        StartCoroutine(GoNextCoroutine());
        return;
    }

    // ＋ボタン（PauseButton → 同じく決定扱い）
    if (Input.GetButtonDown("PauseButton"))
    {
        isTransitioning = true;
        StartCoroutine(GoNextCoroutine());
        return;
    }
}


    IEnumerator GoNextCoroutine()
    {
        // 🔥 先にアニメ停止（重要）
        if (logoAnimator != null)
            logoAnimator.StopAllAnimations();

        // 🔊 SE再生
        if (seSource != null && decideSE != null)
        {
            seSource.PlayOneShot(decideSE);
            yield return new WaitForSeconds(decideSE.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        // 🔥 シーン遷移
        SceneManager.LoadScene(nextScene);
    }
}