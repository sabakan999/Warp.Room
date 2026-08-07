using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BossBattleIntro : MonoBehaviour
{
    [Header("参照（自動取得）")]
    private TimerUI timerUI;
    private BossController bossController;
    private BGMManager bgmManager;
    private AudioSource audioSource;

    [Header("ボス見た目")]
    public GameObject bossVisual;

    [Header("会話")]
    public DialogueUI dialogueUI;

    [Header("ボス顔")]
    public Sprite bossFace;

    [Header("演出時間")]
    public float shakeTime = 0.8f;
    public float chargeTime = 2.5f;
    public float bossAppearDelay = 0.5f;
    public float battleStartDelay = 2f;

    private Vector3 defaultScale;

    [Header("SE")]
    public AudioClip bossAppearSE;

    private bool isPlaying = false;
    private bool waitingDialogue = false;
    private PlayerController playerController;

    string[] introMessages =
    {
        "やぁ！",
        "もしかして君は今\n「もうこれで終わりか。」",
        "なーんて思ったかい？",
        "残念！僕のショー、「WarpRoom」\nはまだ終わらないよ！",
        "なーにそんな顔してるのさ？\n今まで君は楽しんでいたじゃないか！",
        "僕のwarp、\n最高にスリリングだっただろ？",
        "それに久しぶりのお客さんなんだ。\nそう簡単には帰さないさ。",
        "とはいえ今君をここに呼び寄せて\n留めるために僕のwarp魔法はすべて",
        "あのタイマーを増やすのに使ってしまった。\nもうwarpは使えない。",
        "つまりさっきのが正真正銘、\n最後のwarpさ。",
        "さぁ、一世一代の\n最高のショーを始めよう！"
    };

    void Start()
    {
        bossVisual.SetActive(false);
        defaultScale = bossVisual.transform.localScale;

        
         dialogueUI ??= FindFirstObjectByType<DialogueUI>();
        Debug.Log(dialogueUI == null ? "DialogueUI = NULL" : "DialogueUI = " + dialogueUI.name);
    }

    public void StartIntro()
    {
        if (isPlaying) return;

        dialogueUI ??= FindFirstObjectByType<DialogueUI>();
        playerController ??= FindFirstObjectByType<PlayerController>();

        timerUI ??= FindFirstObjectByType<TimerUI>();
        bossController ??= FindFirstObjectByType<BossController>();
        bgmManager ??= FindFirstObjectByType<BGMManager>();

        if (audioSource == null)
        {
            GameObject se = GameObject.Find("SE");

            if (se != null)
                audioSource = se.GetComponent<AudioSource>();
        }

        Debug.Log("===== BossBattleIntro =====");

        isPlaying = true;

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        //--------------------------------------------------
        // ① 時計を震わせる
        //--------------------------------------------------
        if (timerUI != null)
            timerUI.PlayShake(shakeTime);

        yield return new WaitForSeconds(shakeTime);

        //--------------------------------------------------
        // ② 時計を空→満タンへ
        //--------------------------------------------------
        if (timerUI != null)
            yield return timerUI.PlayBossCharge(chargeTime);

        //--------------------------------------------------
        // ③ 時計を赤色へ
        //--------------------------------------------------
        if (timerUI != null)
            timerUI.SetBossMode();

        yield return new WaitForSeconds(0.3f);

        //--------------------------------------------------
        // ④ ボス登場
        //--------------------------------------------------
        if (bossVisual != null)
        {
            bossVisual.SetActive(true);

            bossVisual.transform.localScale = Vector3.zero;

            bossVisual.transform
                .DOScale(defaultScale * 1.2f, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    bossVisual.transform.DOScale(defaultScale, 0.1f);
                });

            BossVisualController visual =
                bossVisual.GetComponent<BossVisualController>();

            bossController.SetBossVisual(visual);
        }

        if (audioSource != null && bossAppearSE != null)
            audioSource.PlayOneShot(bossAppearSE);

        yield return new WaitForSeconds(bossAppearDelay);

        //--------------------------------------------------
        // ⑤ ボス会話
        //--------------------------------------------------
        if (dialogueUI != null)
        {
            if (bossController != null && bossController.bossVisual != null)
            {
                bossController.bossVisual.MoveToTalkPosition();
            }

            if(playerController != null)
            {
                playerController.DisableControl();
            }
             Debug.Log("会話開始");
            dialogueUI.gameObject.SetActive(true);

            dialogueUI.StartDialogue(
                bossFace,
                "アルドラ",
                introMessages
            );

            waitingDialogue = true;

            yield return new WaitUntil(() => waitingDialogue == false);

            dialogueUI.gameObject.SetActive(false);
            
            if(playerController != null)
            {
                playerController.EnableControl();
            }
        }

        //--------------------------------------------------
        // ⑥ ボスBGM
        //--------------------------------------------------
        if (bgmManager != null)
            bgmManager.PlayBossBGM();

        yield return new WaitForSeconds(battleStartDelay);

                if (timerUI != null)
        {
            timerUI.StartBossTimer(60f);
        }

        //--------------------------------------------------
        // ⑦ 戦闘開始
        //--------------------------------------------------
        if (bossController != null)
            bossController.BeginBattle();

        isPlaying = false;
    }

    void Update()
{
    if (!waitingDialogue)
        return;

    // Aボタン（会話送り）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))   // ← Aボタン対応
    {
        bool finished = dialogueUI.NextMessage();

        if (finished)
        {
            waitingDialogue = false;
        }
    }
}
}