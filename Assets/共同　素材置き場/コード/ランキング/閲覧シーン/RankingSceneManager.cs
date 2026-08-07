using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankingSceneManager : MonoBehaviour
{
    [Header("ScrollView")]
    public Transform content;
    public GameObject entryPrefab;

    [Header("表示件数")]
    public int displayCount = 50;

    [Header("スクロール")]
    public ScrollRect scrollRect;
    public float scrollSpeed = 2.5f;

    [Header("演出")]
    public RectTransform contentRect;

    [Header("現在TOP表示")]
    public GameObject topPanel;
    public RectTransform topPanelRect;
    public Text topNameText;
    public Text topScoreText;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip scrollSE;
    public AudioClip topSE;

    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip rankingBGM;

public float introDistance = 3000f;
public float introTime = 2.5f;

    //=========================
    // ランキング
    //=========================
    private List<RankingEntry> rankingList =
        new List<RankingEntry>();

    // 生成したEntryを保存
    private List<GameObject> entryObjects =
        new List<GameObject>();

    // 演出終了後だけ操作可能
    private bool canControl = false;

    void Start()
{
    if (topPanel != null)
        topPanel.SetActive(false);

    RankingAPI.Instance.GetRanking(() =>
    {
        LoadRanking();
        CreateRanking();
        StartCoroutine(IntroAnimation());
    });
}

    void Update()
    {
        if (!canControl)
            return;

        HandleScroll();
    }

    void HandleScroll()
{
    if (scrollRect == null)
        return;

    float move = 0f;

    // キーボード
    if (Input.GetKey(KeyCode.UpArrow))
        move = scrollSpeed;

    if (Input.GetKey(KeyCode.DownArrow))
        move = -scrollSpeed;

    // コントローラー（左スティック）
    float v = Input.GetAxis("Vertical");
    move += v * scrollSpeed;

    scrollRect.verticalNormalizedPosition =
        Mathf.Clamp01(
            scrollRect.verticalNormalizedPosition +
            move * Time.deltaTime
        );
}
    void LoadRanking()
    {
        if (RankingManager.Instance == null)
        {
            Debug.LogError("RankingManagerがシーンにありません！");
            return;
        }

        rankingList = RankingManager.Instance.rankings;

        while (rankingList.Count < displayCount)
        {
            rankingList.Add(
                new RankingEntry("-----", 0)
            );
        }
    }

    void CreateRanking()
    {
        entryObjects.Clear();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < displayCount; i++)
        {
            GameObject obj =
                Instantiate(entryPrefab, content);

            RankingEntryUI ui =
                obj.GetComponent<RankingEntryUI>();

            ui.SetData(
                i + 1,
                rankingList[i].playerName,
                rankingList[i].score,
                i == 0,
                i == 1,
                i == 2
            );

            // 最初は全部非表示
            

            // 後で演出に使う
            entryObjects.Add(obj);
        }
    }
   IEnumerator IntroAnimation()
{
    canControl = false;

    // レイアウト完成待ち
    yield return null;
    Canvas.ForceUpdateCanvases();

    // 一番下（50位）を表示
    scrollRect.verticalNormalizedPosition = 0f;

    yield return new WaitForSeconds(0.3f);

    if(audioSource!=null && scrollSE!=null)
    audioSource.clip = scrollSE;
    audioSource.loop = true;
    audioSource.Play();
    

    // 50位 → 1位へスクロール
    Tween scrollTween =
DOTween.To(
    () => scrollRect.verticalNormalizedPosition,
    value => scrollRect.verticalNormalizedPosition = value,
    1f,
    introTime
).SetEase(Ease.Linear);

   yield return scrollTween.WaitForCompletion();
    

    if(audioSource!=null && topSE!=null)
    audioSource.Stop();
    audioSource.loop = false;
    audioSource.PlayOneShot(topSE);

    if(topPanel!=null)
    {
        topPanel.SetActive(true);

        topNameText.text = rankingList[0].playerName;
        topScoreText.text = rankingList[0].score.ToString();

        topPanelRect.localScale = Vector3.zero;

        topPanelRect
            .DOScale(1.25f,0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(()=>
            {
                topPanelRect.DOScale(1f,0.1f);

                if (bgmSource != null && rankingBGM != null)
                {
                    bgmSource.clip = rankingBGM;
                    bgmSource.loop = true;
                    bgmSource.Play();
                }
            });
    }

    canControl = true;
}
   
}