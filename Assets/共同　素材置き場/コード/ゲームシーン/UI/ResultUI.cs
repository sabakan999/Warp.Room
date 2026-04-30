using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultUI : MonoBehaviour
{
    [Header("通常パネル")]
    public GameObject panel;
    public RectTransform retryText;
    public RectTransform stageSelectText;

    [Header("無限パネル")]
    public GameObject endlessPanel;
    public RectTransform endlessRetryText;
    public RectTransform endlessBackText;
    public Text endlessCountText;

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

    private Text endlessRetryLabel;
    private Text endlessBackLabel;

    private int selectedIndex = 0;
    private float prevH = 0f;
    private bool isActive = false;
    private bool isDeciding = false;

    public BGMManager bgmManager;

    private bool isEndless = false;

    // 🔥 追加：無限スコア保持
    private int endlessScore = 0;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (endlessPanel != null) endlessPanel.SetActive(false);

        retryLabel = retryText.GetComponent<Text>();
        stageLabel = stageSelectText.GetComponent<Text>();

        endlessRetryLabel = endlessRetryText.GetComponent<Text>();
        endlessBackLabel = endlessBackText.GetComponent<Text>();
    }

    void Update()
    {
        if (!isActive || isDeciding) return;

        HandleMove();
        HandleSubmit();
    }

    // =========================
    // 🔥 GameManagerから呼ぶ用
    // =========================
    public void SetEndlessResult(int score)
    {
        endlessScore = score;
    }

    // =========================
    // 表示
    // =========================
    public void Show()
    {
        isEndless = GameSettings.isEndlessMode;

        if (bgmManager != null)
            bgmManager.PlayResultBGM();

        selectedIndex = 0;
        prevH = 0f;
        isActive = true;
        isDeciding = false;

        if (panel != null) panel.SetActive(false);
        if (endlessPanel != null) endlessPanel.SetActive(false);

        if (!isEndless)
        {
            panel.SetActive(true);
            UpdateNormalSelection(true);
        }
        else
        {
            endlessPanel.SetActive(true);

            // 🔥 修正：GameSettingsじゃなく自分の値使う
            if (endlessCountText != null)
                endlessCountText.text = endlessScore.ToString();

            UpdateEndlessSelection(true);
        }
    }

    // =========================
    // 入力
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
    // 見た目
    // =========================
    void UpdateSelection(bool instant)
    {
        if (!isEndless)
            UpdateNormalSelection(instant);
        else
            UpdateEndlessSelection(instant);
    }

    void UpdateNormalSelection(bool instant)
    {
        float retryScale = (selectedIndex == 0) ? selectedScale : normalScale;
        float stageScale = (selectedIndex == 1) ? selectedScale : normalScale;

        if (retryLabel != null)
            retryLabel.color = (selectedIndex == 0) ? selectedColor : normalColor;

        if (stageLabel != null)
            stageLabel.color = (selectedIndex == 1) ? selectedColor : normalColor;

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

    void UpdateEndlessSelection(bool instant)
    {
        float retryScale = (selectedIndex == 0) ? selectedScale : normalScale;
        float backScale = (selectedIndex == 1) ? selectedScale : normalScale;

        if (endlessRetryLabel != null)
            endlessRetryLabel.color = (selectedIndex == 0) ? selectedColor : normalColor;

        if (endlessBackLabel != null)
            endlessBackLabel.color = (selectedIndex == 1) ? selectedColor : normalColor;

        if (instant)
        {
            endlessRetryText.localScale = Vector3.one * retryScale;
            endlessBackText.localScale = Vector3.one * backScale;
        }
        else
        {
            endlessRetryText.DOKill();
            endlessBackText.DOKill();

            endlessRetryText.DOScale(retryScale, scaleTime).SetEase(Ease.OutBack);
            endlessBackText.DOScale(backScale, scaleTime).SetEase(Ease.OutBack);
        }
    }

    // =========================
    // 決定
    // =========================
    System.Collections.IEnumerator DecideCoroutine()
    {
        isDeciding = true;

        PlaySE(decideSE);

        float wait = (decideSE != null) ? decideSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        if (selectedIndex == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("モードセレクト");
        }
    }

    // =========================
    // SE
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}