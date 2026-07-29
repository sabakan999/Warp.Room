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

    public enum TutorialStep
    {
        Intro,
        Move,
        Jump,
        Jumpplay,
        Timer,
        Timerplay,
        End
    }

    public TutorialStep currentStep = TutorialStep.Intro;

    void Start()
    {
        missionUI.Hide();

        if (moveTarget != null)
            moveTarget.SetActive(false);

        if (jumpTarget != null)
            jumpTarget.SetActive(false);

        StartStep(currentStep);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
                        "ディモールト！",
                        "素晴らしいジャンプ！",
                        "次はゲームのルールについて説明するよ。",
                        "このゲームは次々とWarpする部屋を",
                        "3秒間生き残るゲームなんだ！",
                        "実際にWarpを体験してみよう！"
                    }
                );

                break;

            case TutorialStep.Timerplay:

                SetPlayerControl(true);

                missionUI.Show("3秒間生き残ろう！");

                tutorialWarpManager.StartWarpTimer();

                break;

            case TutorialStep.End:

                SetPlayerControl(false);

                dialogueUI.StartDialogue(
                    guideFace,
                    "ガイド",
                    new string[]
                    {
                        "今のがWarpだよ！",
                        "3秒経つと部屋が切り替わるんだ。",
                        "次は敵について説明するよ！"
                    }
                );

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
}