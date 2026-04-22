using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultUI : MonoBehaviour
{
    [Header("パネル")]
    public GameObject panel;

    [Header("選択テキスト")]
    public RectTransform retryText;
    public RectTransform stageSelectText;

    [Header("色設定")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("拡大設定")]
    public float selectedScale = 1.2f;
    public float normalScale = 1.0f;
    public float scaleTime = 0.15f;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;

    private Text retryLabel;
    private Text stageLabel;

    private int selectedIndex = 0;
    private float prevH = 0f;
    private bool isActive = false;
    private bool isDeciding = false;

    public BGMManager bgmManager;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        // 🔥 Text取得
        retryLabel = retryText.GetComponent<Text>();
        stageLabel = stageSelectText.GetComponent<Text>();
    }

    void Update()
    {
        if (!isActive || isDeciding) return;

        HandleMove();
        HandleSubmit();
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);

         if (bgmManager != null)
        bgmManager.PlayResultBGM();


        isActive = true;
        isDeciding = false;
        selectedIndex = 0;

        UpdateSelection(true);
    }

    // =========================
    // 🎮 入力処理
    // =========================
    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");

        int prevIndex = selectedIndex;

        if (h > 0.5f && prevH <= 0.5f)
            selectedIndex++;
        else if (h < -0.5f && prevH >= -0.5f)
            selectedIndex--;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, 1);
        prevH = h;

        if (prevIndex != selectedIndex)
        {
            PlaySE(moveSE);
            UpdateSelection(false);
        }
    }

    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(DecideCoroutine());
        }
    }

    // =========================
    // 🎯 見た目更新
    // =========================
    void UpdateSelection(bool instant)
    {
        float retryScale = (selectedIndex == 0) ? selectedScale : normalScale;
        float stageScale = (selectedIndex == 1) ? selectedScale : normalScale;

        // 🔥 色変更
        if (retryLabel != null)
            retryLabel.color = (selectedIndex == 0) ? selectedColor : normalColor;

        if (stageLabel != null)
            stageLabel.color = (selectedIndex == 1) ? selectedColor : normalColor;

        // 🔥 拡大
        if (instant)
        {
            retryText.localScale = Vector3.one * retryScale;
            stageSelectText.localScale = Vector3.one * stageScale;
        }
        else
        {
            retryText.DOKill();
            stageSelectText.DOKill();

            retryText.DOScale(retryScale, scaleTime).SetEase(Ease.OutBack);
            stageSelectText.DOScale(stageScale, scaleTime).SetEase(Ease.OutBack);
        }
    }

    // =========================
    // 🔥 決定処理（音待ち）
    // =========================
    System.Collections.IEnumerator DecideCoroutine()
    {
        isDeciding = true;

        PlaySE(decideSE);

        float wait = 0.2f;
        if (decideSE != null)
            wait = decideSE.length;

        yield return new WaitForSeconds(wait);

        if (selectedIndex == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("ステージセレクト");
        }
    }

    // =========================
    // 🔊 SE
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}