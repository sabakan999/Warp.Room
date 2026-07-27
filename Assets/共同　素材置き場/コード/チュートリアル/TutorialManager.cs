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

    [Header("チュートリアル用オブジェクト")]
    public GameObject moveTarget;
    public GameObject JumpTarget;

    public enum TutorialStep
    {
        Intro,
        Move,
        Jump,
        Jumpplay,
        Timer,
        End
    }

    public TutorialStep currentStep = TutorialStep.Intro;

    void Start()
    {
        missionUI.Hide();

        if (moveTarget != null)
            moveTarget.SetActive(false);
         if (JumpTarget != null)
            JumpTarget.SetActive(false);

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
    public void ReportJump()
    {
        if (currentStep != TutorialStep.Jumpplay)
            return;

        missionUI.Hide();

        if (JumpTarget != null)
            JumpTarget.SetActive(false);

        SetPlayerControl(false);

        NextStep();
    }

    void SetPlayerControl(bool enable)
    {
        if (player != null)
            player.enabled = enable;
    }

    void StartStep(TutorialStep step)
    {
        Debug.Log("現在のSTEP : " + step);

        Debug.Log(moveTarget);

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
                        "このゲームの遊び方を説明するよ！",
                        "まずは移動から始めよう！"
                    }
                );

                break;

            case TutorialStep.Move:

                SetPlayerControl(true);

                if (moveTarget != null)
                    moveTarget.SetActive(true);

                missionUI.Show("光る玉を取りに行こう！");

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

                if (JumpTarget != null)
                    JumpTarget.SetActive(true);

                missionUI.Show("光る玉を取りに行こう！");

                
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
                            "次はゲームのルールについて説明するよ。"
                        }
                );

                break;

            case TutorialStep.End:

                break;
        }
    }

    //==============================
    // 移動ミッション達成
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
}