using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BossClearSequence : MonoBehaviour
{
    [Header("UI")]
    public DialogueUI dialogueUI;

    [Header("ボス顔")]
    public Sprite bossFace;


    [Header("幕")]
    public RectTransform curtain;
    public Vector2 curtainStart;
    public Vector2 curtainEnd;
    public float curtainTime = 1.5f;


    [Header("エンドロール")]
    public GameObject endRoll;
    public EndRollManager endRollManager;

    


    [Header("BGM")]
    public BGMManager bgmManager;



    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip fingerSnapSE;



    



    private bool waitingInput = false;

    private PlayerController playerController;



    string[] bossMessages =
    {
        "...",

        "...おや？",

        "どうやら僕のWarpエネルギーを\n全て使い切ってしまったようだ。",

        "タイマーをムリヤリ動かすのに僕の\nWarpエネルギーは全て使ってしまったからね。",
        "タイマーが0になってしまえば\n君はこの部屋から簡単に出られる。",


        "ははっ！完敗だ！",

        "まさか本当にこのショーを\n最後までこなしてしまうとは！",

        "君は最高のwarperだな！",

        "君のような素晴らしいお客さんと\nこうしてショーができたんだ！",

        "僕は満足さ！",

        "...え？",

        "勝手にこんなところへ連れてきて\n迷惑だった？",

        "ははっ！それは失礼！",

        "いやぁ、100年ぶりに\nお客さんが近くに来てくれたものでね。",

        "なんせ僕はここから\n出られないんだ。",

        "僕の力を危険に思った人間が\nここへ閉じ込めてしまったのさ。",

        "僕はただ、ショーが\nしたかっただけなのにね。",

        "だから、僕と\nあそんでくれてありがとう！",

        "またいつでも来てくれたまえ！",
        "…",

        "…おっと、カーテンコールを\n忘れるところだった。",

        "最高のショーには\n最高のフィナーレがお似合いさ！"
    };





    void Start()
    {
        // 通常プレイ中は非表示

        if(curtain != null)
            curtain.gameObject.SetActive(false);


        if(endRoll != null)
            endRoll.SetActive(false);

        

        

       
    }






    public void StartClear()
    {
        StartCoroutine(ClearRoutine());
    }





    IEnumerator ClearRoutine()
    {
        playerController ??= FindFirstObjectByType<PlayerController>();


        // ボスBGM停止
        if(bgmManager != null)
        {
            bgmManager.StopBGM();
        }


        // 勝利後の静寂
        yield return new WaitForSeconds(3f);

       

        


            if(playerController != null)
            {
                playerController.DisableControl();
            }
        // 会話UI表示
        if(dialogueUI != null)
            dialogueUI.gameObject.SetActive(true);



        dialogueUI.StartDialogue(
            bossFace,
            "Warpエンターテイナー",
            bossMessages
        );


        waitingInput = true;
    }





    void Update()
{
    if (!waitingInput)
        return;

    // Aボタン（会話送り）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))   // ← Aボタン対応
    {
        bool finished = dialogueUI.NextMessage();

        if (finished)
        {
            waitingInput = false;
            StartCoroutine(Finale());
        }
    }
}






    IEnumerator Finale()
    {
        // 指パッチン

        if(audioSource != null && fingerSnapSE != null)
        {
            audioSource.PlayOneShot(fingerSnapSE);
        }



        yield return new WaitForSeconds(0.5f);





        // 幕を降ろす

        if(curtain != null)
        {
            curtain.gameObject.SetActive(true);


            curtain.anchoredPosition = curtainStart;


            curtain
                .DOAnchorPos(
                    curtainEnd,
                    curtainTime
                )
                .SetEase(Ease.OutCubic);
        }



        yield return new WaitForSeconds(curtainTime);





        // エンドロールBGM開始

        if(bgmManager != null)
        {
            bgmManager.PlayEndRollBGM();
        }





        // エンドロール開始

        if(endRollManager != null)
        {
            endRollManager.StartEndRoll();
        }
        else
        {
            Debug.LogWarning(
                "EndRollManagerが設定されていません"
            );
        }
    }





   
}