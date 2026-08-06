using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class EndRollManager : MonoBehaviour
{
    [Header("エンドロールパネル")]
    public GameObject endRollPanel;


    [Header("エンドロール文字")]
    public RectTransform rollText;


    [Header("スクロール設定")]
    public float startY = -1000f;
    public float endY = 3000f;
    public float scrollTime = 30f;


    [Header("終了フェード")]
    public CanvasGroup fadeCanvas;
    public float fadeTime = 1f;


    [Header("遷移先")]
    public string titleScene = "タイトル";


    private bool finished = false;
    private bool isEnding = false;



    void Start()
    {
        // 最初は非表示
        if (endRollPanel != null)
            endRollPanel.SetActive(false);


        // フェード初期化
        if(fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.gameObject.SetActive(true);
        }
    }



    public void StartEndRoll()
    {
        if (endRollPanel == null)
        {
            Debug.LogWarning("EndRollPanelが設定されていません");
            return;
        }


        finished = false;


        // 表示
        endRollPanel.SetActive(true);


        // 初期位置
        rollText.anchoredPosition =
            new Vector2(
                rollText.anchoredPosition.x,
                startY
            );


        rollText.DOKill();


        // スクロール開始
        rollText
            .DOAnchorPosY(
                endY,
                scrollTime
            )
            .SetEase(Ease.Linear)
            .SetLink(rollText.gameObject)
            .OnComplete(() =>
            {
                finished = true;
            });
    }





    void Update()
    {
        if (!finished || isEnding)
            return;


        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ReturnTitle());
        }
    }





    IEnumerator ReturnTitle()
    {
        isEnding = true;


        if(fadeCanvas != null)
        {
            yield return fadeCanvas
                .DOFade(
                    1f,
                    fadeTime
                )
                .SetEase(Ease.InQuad)
                .SetLink(fadeCanvas.gameObject)
                .WaitForCompletion();
        }


        yield return new WaitForSeconds(0.2f);


        SceneManager.LoadScene(titleScene);
    }





    void OnDisable()
    {
        if (rollText != null)
        {
            rollText.DOKill();
        }


        if(fadeCanvas != null)
        {
            fadeCanvas.DOKill();
        }
    }
}