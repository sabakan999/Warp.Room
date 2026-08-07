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

    [Header("ランキング（1～3位）")]
    public Text firstText;
    public Text secondText;
    public Text thirdText;


    [Header("4～10位")]
    public Text rankingText;

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

    public void SetEndlessResult(int score)
    {
        endlessScore = score;
    }

    public void Show()
    {
        isEndless = GameSettings.isEndlessMode;

        if (bgmManager != null)
            bgmManager.PlayResultBGM();

        selectedIndex = 0;
        prevH = 0f;
        isActive = true;
        isDeciding = false;

        if (panel != null)
            panel.SetActive(false);

        if (endlessPanel != null)
            endlessPanel.SetActive(false);

        if (!isEndless)
        {
            panel.SetActive(true);
            UpdateNormalSelection(true);
        }
        else
        {
            endlessPanel.SetActive(true);

            if (endlessCountText != null)
                endlessCountText.text = endlessScore.ToString();

            // ランキング登録
            if (RankingManager.Instance != null)
            {
                // 初回プレイ
                if (!RetryData.isRetry)
                {
                    RankingAPI.Instance.PostScore(
                        GameSettings.playerName,
                        endlessScore,
                        () =>
                        {
                            RankingAPI.Instance.GetRanking(() =>
                            {
                                RefreshRanking();
                            });
                        });
                }
                // リトライ中
                else
                {
                     Debug.Log("旧スコア：" + RetryData.bestScore);
                     Debug.Log("今回：" + endlessScore);
                    // 前回以下なら登録しない
                    if (endlessScore <= RetryData.bestScore)
                    {
                        RankingAPI.Instance.GetRanking(() =>
                        {
                            RefreshRanking();
                        });
                    }
                    // 更新した
                    else
                    {
                       RankingAPI.Instance.DeleteScore(
                        RetryData.playerName,
                        RetryData.bestScore,
                        () =>
                        {
                            RankingAPI.Instance.PostScore(
                                RetryData.playerName,
                                endlessScore,
                                () =>
                                {
                                    RetryData.bestScore = endlessScore;

                                    RankingAPI.Instance.GetRanking(() =>
                                    {
                                        RefreshRanking();
                                    });
                                });
                        });
                    }
                }
            }

            UpdateEndlessSelection(true);
        }
    }

    

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
    // Aボタン（決定）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        StartCoroutine(DecideCoroutine());
        return;
    }

    
}


void OnBack()
{
    if (!isEndless)
    {
        // 通常モード → ステージセレクトへ
        SceneManager.LoadScene("ステージセレクト");
    }
    else
    {
        // エンドレス → Back を選んだのと同じ動作
        RetryData.isRetry = false;
        RetryData.playerName = "";
        RetryData.bestScore = 0;

        SceneManager.LoadScene("モードセレクト");
    }
}

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

            retryText
                .DOScale(retryScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(retryText.gameObject);

            stageSelectText
                .DOScale(stageScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(stageSelectText.gameObject);
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

            endlessRetryText
                .DOScale(retryScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(endlessRetryText.gameObject);

            endlessBackText
                .DOScale(backScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(endlessBackText.gameObject);
        }
    }

        System.Collections.IEnumerator DecideCoroutine()
    {
        isDeciding = true;

        PlaySE(decideSE);

        float wait = (decideSE != null) ? decideSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        if (!isEndless)
        {
            if (selectedIndex == 0)
            {
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().name
                );
            }
            else
            {
                SceneManager.LoadScene("ステージセレクト");
            }
        }
        else
        {
            if (selectedIndex == 0)
            {   
                 if (!RetryData.isRetry)
                {
                    RetryData.playerName = GameSettings.playerName;
                    RetryData.bestScore = endlessScore;
                }
                else
                {
                    RetryData.bestScore = Mathf.Max(RetryData.bestScore, endlessScore);
                }
                RetryData.isRetry = true;
                

                Debug.Log("保存した");
                Debug.Log("name = " + RetryData.playerName);
                Debug.Log("score = " + RetryData.bestScore);
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().name
                );
            }
            else
            {
                RetryData.isRetry = false;
                RetryData.playerName = "";
                RetryData.bestScore = 0;
                SceneManager.LoadScene("モードセレクト");
            }
        }
    }

    void RefreshRanking()
    {
        

        if (RankingAPI.Instance == null)
        return;

        var list = RankingAPI.Instance.rankings;
        //------------------------
        // 1位
        //------------------------
        if (list.Count >= 1)
        {
           firstText.text =
    list[0].score+"warp"+
    " " + list[0].playerName
    ;
        }

        //------------------------
        // 2位
        //------------------------
        if (list.Count >= 2)
        {
            secondText.text =
    list[1].score+
    "warp"+" " +
    list[1].playerName ;
        }

        //------------------------
        // 3位
        //------------------------
        if (list.Count >= 3)
        {
           thirdText.text =
    list[2].score+"warp"+" " +list[2].playerName
    ;
        }

        //------------------------
        // 4～10位
        //------------------------
        if (rankingText != null)
        {
            rankingText.text = "";

            for (int i = 3; i < 10; i++)
            {
                if (i < list.Count)
                {
                    rankingText.text +=
                        (i + 1) +
                        ". " +
                         list[i].playerName+
                        " " +
                        list[i].score
                        ;
                }
                else
                {
                    rankingText.text +=
                        (i + 1) +
                        ". -----";
                }

                if (i== 6)
                    rankingText.text += "\n";
                else
                    rankingText.text += "　";
                
                
            }
        }
    }

    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}