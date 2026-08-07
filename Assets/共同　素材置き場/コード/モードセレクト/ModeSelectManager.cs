using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class ModeSelectManager : MonoBehaviour
{
    [Header("選択肢UI")]
    public RectTransform normalButton;
    public RectTransform endlessButton;

    [Header("2ページ目")]
    public GameObject page1;
    public GameObject page2;

    public RectTransform rankingButton;
    public RectTransform settingButton;

    public CanvasGroup rankingFrame;
    public CanvasGroup settingFrame;

    [Header("装飾フレーム")]
    public CanvasGroup normalFrame;
    public CanvasGroup endlessFrame;

    [Header("拡大設定")]
    public float selectedScale = 1.2f;
    public float normalScale = 1.0f;
    public float scaleTime = 0.15f;

    [Header("フレームフェード")]
    public float fadeTime = 0.25f;

    [Header("決定演出")]
public float pressScale = 0.85f;
public float pressTime = 0.08f;

public float popScale = 1.35f;
public float popTime = 0.15f;

    [Header("次シーン")]
    public string normalNextScene = "ステージセレクト";
    public string endlessNextScene = "ワープ・ルーム";
    public string rankingScene = "ランキング";
    public string settingScene = "設定";

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;

    private int selectedIndex = 0; // 0=左 1=右
    private float prevH = 0f;
    private bool isDeciding = false; // 入力ロック

    private int pageIndex = 0;      //0=ゲーム 1=その他
    private float prevV = 0f;

    void Start()
    {
        UpdateSelection(true);
        UpdateFrame(true);
        UpdatePage(true);

        
    }

    void Update()
    {
        if (isDeciding) return;
        HandleMove();
        HandlePageMove();
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
    if (pageIndex == 0)
    {
        UpdateGameSelection(instant);
    }
    else
    {
        UpdateOtherSelection(instant);
    }
}

void UpdateGameSelection(bool instant)
{
    float normalScale =
        (selectedIndex == 0) ? selectedScale : this.normalScale;

    float endlessScale =
        (selectedIndex == 1) ? selectedScale : this.normalScale;

    if (instant)
    {
        normalButton.localScale = Vector3.one * normalScale;
        endlessButton.localScale = Vector3.one * endlessScale;
    }
    else
    {
        normalButton.DOKill();
        endlessButton.DOKill();

        normalButton
            .DOScale(normalScale, scaleTime)
            .SetEase(Ease.OutBack)
            .SetLink(normalButton.gameObject);

        endlessButton
            .DOScale(endlessScale, scaleTime)
            .SetEase(Ease.OutBack)
            .SetLink(endlessButton.gameObject);
    }
}

void UpdateOtherSelection(bool instant)
{
    float rankingScale =
        (selectedIndex == 0) ? selectedScale : normalScale;

    float settingScale =
        (selectedIndex == 1) ? selectedScale : normalScale;

    if (instant)
    {
        rankingButton.localScale = Vector3.one * rankingScale;
        settingButton.localScale = Vector3.one * settingScale;
    }
    else
    {
        rankingButton.DOKill();
        settingButton.DOKill();

        rankingButton
            .DOScale(rankingScale, scaleTime)
            .SetEase(Ease.OutBack)
            .SetLink(rankingButton.gameObject);

        settingButton
            .DOScale(settingScale, scaleTime)
            .SetEase(Ease.OutBack)
            .SetLink(settingButton.gameObject);
    }
}

    // =========================
    // 🎨 フレーム
    // =========================
   void UpdateFrame(bool instant)
{
    if (pageIndex == 0)
    {
        UpdateGameFrame(instant);
    }
    else
    {
        UpdateOtherFrame(instant);
    }
}

void UpdateGameFrame(bool instant)
{
    float normalAlpha =
        (selectedIndex == 0) ? 1f : 0f;

    float endlessAlpha =
        (selectedIndex == 1) ? 1f : 0f;

    if (instant)
    {
        normalFrame.alpha = normalAlpha;
        endlessFrame.alpha = endlessAlpha;
    }
    else
    {
        normalFrame.DOKill();
        endlessFrame.DOKill();

        normalFrame
            .DOFade(normalAlpha, fadeTime)
            .SetEase(Ease.OutQuad)
            .SetLink(normalFrame.gameObject);

        endlessFrame
            .DOFade(endlessAlpha, fadeTime)
            .SetEase(Ease.OutQuad)
            .SetLink(endlessFrame.gameObject);
    }
}

void UpdateOtherFrame(bool instant)
{
    float rankingAlpha =
        (selectedIndex == 0) ? 1f : 0f;

    float settingAlpha =
        (selectedIndex == 1) ? 1f : 0f;

    if (instant)
    {
        rankingFrame.alpha = rankingAlpha;
        settingFrame.alpha = settingAlpha;
    }
    else
    {
        rankingFrame.DOKill();
        settingFrame.DOKill();

        rankingFrame
            .DOFade(rankingAlpha, fadeTime)
            .SetEase(Ease.OutQuad)
            .SetLink(rankingFrame.gameObject);

        settingFrame
            .DOFade(settingAlpha, fadeTime)
            .SetEase(Ease.OutQuad)
            .SetLink(settingFrame.gameObject);
    }
}

    // =========================
    // 🎮 決定
    // =========================
   void HandleSubmit()
{
    // Aボタン（決定）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        StartCoroutine(SelectModeCoroutine());
        return;
    }



   
}


    IEnumerator SelectModeCoroutine()
    {
        isDeciding = true;

    

        // 🔊 決定音
        PlaySE(decideSE);

         yield return StartCoroutine(PlayDecisionEffect());

        // 🔥 音が鳴り終わるまで待つ
        float waitTime = 0.2f;

        if (decideSE != null)
            waitTime = decideSE.length;

        yield return new WaitForSeconds(waitTime);

       if (pageIndex == 0)
{
    //==================
    // ゲームページ
    //==================

    if (selectedIndex == 0)
    {
        // ノーマル
        GameSettings.isEndlessMode = false;
        SceneManager.LoadScene(normalNextScene);
    }
    else
    {
        // エンドレス
        GameSettings.isEndlessMode = true;

        // 名前入力へ
        SceneManager.LoadScene(endlessNextScene);
    }
}
else
{
    //==================
    // その他ページ
    //==================

    if (selectedIndex == 0)
    {
        // ランキング
        SceneManager.LoadScene(rankingScene);
    }
    else
    {
        // 設定
        SceneManager.LoadScene(settingScene);
    }
}
    }

RectTransform GetSelectedButton()
{
    if (pageIndex == 0)
    {
        // ゲームページ
        return selectedIndex == 0
            ? normalButton
            : endlessButton;
    }
    else
    {
        // その他ページ
        return selectedIndex == 0
            ? rankingButton
            : settingButton;
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

    void HandlePageMove()
{
    float v = Input.GetAxisRaw("Vertical");

    int prevPage = pageIndex;

    if (v > 0.5f && prevV <= 0.5f)
        pageIndex--;

    else if (v < -0.5f && prevV >= -0.5f)
        pageIndex++;

    pageIndex = Mathf.Clamp(pageIndex, 0, 1);

    prevV = v;

    if (prevPage != pageIndex)
    {
        selectedIndex = 0;

        PlaySE(moveSE);

        UpdatePage(false);
        UpdateSelection(true);
        UpdateFrame(true);
    }
}

void UpdatePage(bool instant)
{
    if(page1!=null)
        page1.SetActive(pageIndex==0);

    if(page2!=null)
        page2.SetActive(pageIndex==1);
}
IEnumerator PlayDecisionEffect()
{
    RectTransform target = GetSelectedButton();

    if (target == null)
        yield break;


    target.DOKill();


    // 押し込む
    yield return target
        .DOScale(pressScale, pressTime)
        .SetEase(Ease.OutQuad)
        .SetLink(target.gameObject)
        .WaitForCompletion();


    // 弾むように拡大
    yield return target
        .DOScale(popScale, popTime)
        .SetEase(Ease.OutBack)
        .SetLink(target.gameObject)
        .WaitForCompletion();


    // 少し待つ
    yield return new WaitForSeconds(0.05f);
}
}