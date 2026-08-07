using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public DialogueUI dialogueUI;
    public MissionUI missionUI;

    [Header("ガイド")]
    public Sprite guideFace;

    [Header("プレイヤー")]
    public PlayerController player;

    [Header("チュートリアルシステム")]
    public TutorialWarpManager tutorialWarpManager;

    [Header("チュートリアル用オブジェクト")]
    public GameObject moveTarget;
    public GameObject jumpTarget;
    public TutorialEnemySpawner enemySpawner;
    public TutorialBarrierSpawner barrierSpawner;
    public TutorialCurseSpawner CurseSpawner;
    public TutorialEndingManager endingManager;
    public TutorialObjectUI tutorialArrow;
    
    [Header("死亡演出")]
    public AudioSource deathSESource;
    
    public GameObject deathEffectPrefab;

    public enum TutorialStep
    {
        Intro,
        Move,
        Jump,
        Jumpplay,
        Timer,
        Timerplay,
        Enemyexplain,
        Enemyplay,
        Enemyresult,
        respawn,
        barrier,
        barrierplay,
        curse,      
        End,
        Ending
    }

    public TutorialStep currentStep = TutorialStep.Intro;

    void Start()
    {
        missionUI.Hide();
        tutorialArrow.Hide();

        if (moveTarget != null)
            moveTarget.SetActive(false);

        if (jumpTarget != null)
            jumpTarget.SetActive(false);

        StartStep(currentStep);
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))   // ← Aボタン対応
    {
        if (Time.timeScale == 0f)
            return;

        if (dialogueUI.IsTalking)
        {
            bool finished = dialogueUI.NextMessage();

            if (finished)
            {
                NextStep();
            }
        }
    }
}

    public void NextStep()
    {
        currentStep++;

        if ((int)currentStep >= System.Enum.GetValues(typeof(TutorialStep)).Length)
            return;

        StartStep(currentStep);
    }

    public void SetPlayer(PlayerController newPlayer)
    {
        player = newPlayer;
    }

    void SetPlayerControl(bool enable)
    {
        if (player != null)
            player.enabled = enable;
    }

    void StartStep(TutorialStep step)
    {
        Debug.Log("現在のSTEP : " + step);

        switch (step)
        {
            case TutorialStep.Intro:

                SetPlayerControl(false);

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "ようこそ！",
                        "初めに操作方法を説明するよ！",
                        "まずは移動から始めよう！"
                    }
                );

                break;

            case TutorialStep.Move:

                SetPlayerControl(true);

                if (moveTarget != null)
                    moveTarget.SetActive(true);

                missionUI.Show("←→入力で移動してフラッグをゲットしよう！");

                break;

            case TutorialStep.Jump:

                SetPlayerControl(false);

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "いいね！",
                        "移動は完璧！",
                        "次はジャンプしてみよう！"
                    }
                );

                break;

            case TutorialStep.Jumpplay:

                SetPlayerControl(true);

                if (jumpTarget != null)
                    jumpTarget.SetActive(true);

                missionUI.Show("スペースキーでジャンプしてフラッグをゲットしよう！");

                break;

            case TutorialStep.Timer:

                SetPlayerControl(false);

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "イイ感じ！",
                        "素晴らしいジャンプ！",
                        "次はWarpRoomの",
                        "ルールについて説明するよ。",
                        "このゲームは次々とWarpする不思議な部屋を",
                        "3秒間生き残ればクリアだよ！",
                        "実際にWarpを体験してみよう！"
                    }
                );

                break;

            case TutorialStep.Timerplay:

                SetPlayerControl(true);
                tutorialArrow.Show();

                missionUI.Show("3秒間生き残ろう！");

                tutorialWarpManager.StartWarpTimer();

                break;

            case TutorialStep.Enemyexplain:

                SetPlayerControl(false);
                tutorialArrow.Hide();

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "今のがWarpだよ！",
                        "3秒経つと部屋が切り替わるんだ。",
                        "次は敵について説明するよ！",
                        "今から出てくる赤いヤツに触ってみよう！"
                    }
                );

                break;

            case TutorialStep.Enemyplay:

                SetPlayerControl(true);
                enemySpawner.SpawnEnemy();

                missionUI.Show("触ってみると...？");
                break;

            case TutorialStep.Enemyresult:

                SetPlayerControl(false);

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "こんな風に赤いヤツに触れると",
                        "やられちゃうから気を付けてね！",
                        "さてと、本編ではできないけど",
                        "今回だけ特別に蘇生させよう。",
                        "えーい！"
                    }
                    
                );
               
                break;
            case TutorialStep.respawn:

                SetPlayerControl(false);
                tutorialWarpManager.StartWarpTimer();
                missionUI.Show("蘇生中...");
                break;
             case TutorialStep.barrier:

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "これでよし。",
                        "最後に少しお得な情報を",
                        "教えちゃうね！",
                        "バリアというアイテムを取得すると",
                        "一度だけ身を守ってくれるよ！",
                        "やってみよう！",
                        
                    }
                                         );
                    
                    break;
                case TutorialStep.barrierplay:
                        SetPlayerControl(true);
                        barrierSpawner.SpawnBarrier();
                        enemySpawner.SpawnEnemy();

                        missionUI.Show("バリアで防いでみよう");
                    break;
                
                case TutorialStep.curse:
                        enemySpawner.DestroyEnemy();
                        CurseSpawner.SpawnCurse();
                        CurseSpawner.SpawnAngel();
                        SetPlayerControl(true);
                         

                        dialogueUI.StartDialogue(
                            guideFace,
                            "ガイド",
                            new string[]
                            {
                                "サイコー！",
                                "今回はやられずに済んだね！",
                                "アイテムはプレイヤーを有利にしてくれる",
                                "効果がほとんどだからドンドンゲットしよう！",
                                "ただし！",
                                "呪いどくろというアイテムは",
                                "プレイヤーを不利にしてしまうので",
                                "注意！",
                                "ゲットすると呪い状態になってしまい",
                                "呪い状態のままワープすると",
                                "やられてしまう！",
                                "エンジェルをゲットすれば解呪できるので",
                                "次のワープまでに急いで",
                                "エンジェルへ向かおう。"
                                
                            }
                                        );
                        break;

                case TutorialStep.End:
                 SetPlayerControl(false);

                    dialogueUI.StartDialogue(
                        guideFace,
                        "ガイド",
                        new string[]
                        {
                            "これでチュートリアルはばっちり！",
                            "きっと本番では新たな仕掛けが",
                            "待ち受けているけど君なら大丈夫！",
                            "何度もリトライしてパターンを覚えよう！",
                            "Have a nice Warp!!"
                            
                            
                        }
                                            );
                        
                        break;

                case TutorialStep.Ending:
                 SetPlayerControl(false);
                 endingManager.PlayEnding();

                    
                        
                        break;
                
        }
    }

    //==============================
    // 移動達成
    //==============================
    public void ReportMove()
    {
        if (currentStep != TutorialStep.Move)
            return;

        missionUI.Hide();

        if (moveTarget != null)
            moveTarget.SetActive(false);

        SetPlayerControl(false);

        NextStep();
    }

    //==============================
    // ジャンプ達成
    //==============================
    public void ReportJump()
    {
        if (currentStep != TutorialStep.Jumpplay)
            return;

        missionUI.Hide();

        if (jumpTarget != null)
            jumpTarget.SetActive(false);

        SetPlayerControl(false);

        NextStep();
    }

    //==============================
    // Warp終了通知
    //==============================
    public void ReportWarpFinished()
    {
        missionUI.Hide();

        SetPlayerControl(false);

        NextStep();
    }
    
    //==============================
    // 死亡処理
    //==============================

   public void ReportPlayerDead(GameObject player)
    {
        if (currentStep != TutorialStep.Enemyplay)
            return;

        StartCoroutine(PlayerDeadRoutine(player));
    }

    IEnumerator PlayerDeadRoutine(GameObject deadPlayer)
    {
        // プレイヤー操作停止
        SetPlayerControl(false);

        // ミッション非表示
        missionUI.Hide();

        // プレイヤーを見えなくする
        if (deadPlayer != null)
        {
            foreach (SpriteRenderer sr in deadPlayer.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }

            // 当たり判定も消す
            foreach (Collider2D col in deadPlayer.GetComponentsInChildren<Collider2D>())
            {
                col.enabled = false;
            }
        }

       

        // 死亡エフェクト
        if (deathEffectPrefab != null && deadPlayer != null)
        {
            Instantiate(
                deathEffectPrefab,
                deadPlayer.transform.position,
                Quaternion.identity
            );
        }

        // 演出待ち
        yield return new WaitForSeconds(0.6f);

        // プレイヤー削除
        if (deadPlayer != null)
            Destroy(deadPlayer);

        // 敵削除
        if (enemySpawner != null)
            enemySpawner.DestroyEnemy();

        // 次の会話へ
        NextStep();
    }

    public void ReportBarrierUsed()
    {
        if (currentStep != TutorialStep.barrierplay)
            return;

        missionUI.Hide();

        if (barrierSpawner != null)
            barrierSpawner.DestroyBarrier();

        if (enemySpawner != null)
            enemySpawner.DestroyEnemy();

        SetPlayerControl(false);

        NextStep();
    }
        }
    
