using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleInput : MonoBehaviour
{
    [Header("遷移先シーン")]
    public string nextScene = "モードセレクト";

    [Header("UIまとめ")]
    public GameObject titleRoot; // タイトルUIや演出の親

    [Header("暗転")]
    public GameObject fadePanel;
    public float fadeTime = 0.5f;
    public float firstDelay = 1f;

    [Header("BGM")]
    public AudioSource bgmSource;

    private bool canInput = false;
    private bool isTransitioning = false;

public TitleTeleportPlayer teleportPlayer;
    void Start()
    {
        // 🔥 最初は全部隠す
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
        // ⏳ 1秒待つ（何もない状態）
        yield return new WaitForSeconds(firstDelay);

        // 🌑 暗転ON
        if (fadePanel != null)
            fadePanel.SetActive(true);

        yield return new WaitForSeconds(fadeTime);

        // ✨ UI＆演出出現
        if (titleRoot != null)
            titleRoot.SetActive(true);

        if (teleportPlayer != null)
             teleportPlayer.StartTeleport();

        // 🎵 BGM開始
        if (bgmSource != null)
            bgmSource.Play();

        // 🌑 暗転OFF
        if (fadePanel != null)
            fadePanel.SetActive(false);

        // 🎮 入力解禁
        canInput = true;
    }

    void Update()
    {
        if (!canInput || isTransitioning)
            return;

        if (Input.GetButtonDown("Submit") ||
            Input.GetKeyDown(KeyCode.Space))
        {
            isTransitioning = true;
            GoNext();
        }
    }

    void GoNext()
    {
        SceneManager.LoadScene(nextScene);
    }
}