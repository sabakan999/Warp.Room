using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
    public StageButton[,] buttons = new StageButton[3, 3];

    public RectTransform cursor;

    [Header("戻る設定")]
    public string modeSelectSceneName = "モードセレクト";

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;
    public AudioClip backSE;

    private int x = 0;
    private int y = 0;

    float prevH = 0f;
    float prevV = 0f;

    bool isTransitioning = false; // 🔥 連打防止

    void Start()
    {
        StageButton[] stageButtons = GetComponentsInChildren<StageButton>();

        int index = 0;
        foreach (StageButton btn in stageButtons)
        {
            int row = index / 3;
            int col = index % 3;

            if (col < 3 && row < 3)
                buttons[row, col] = btn;

            index++;
        }

        UpdateCursor();
    }

    void Update()
    {
        if (isTransitioning) return; // 🔥 遷移中は操作無効

        HandleMove();
        HandleSubmit();
        HandleBack();
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool moved = false;

        if (h > 0.5f && prevH <= 0.5f)
        {
            x++;
            moved = true;
        }
        if (h < -0.5f && prevH >= -0.5f)
        {
            x--;
            moved = true;
        }

        if (v > 0.5f && prevV <= 0.5f)
        {
            y--;
            moved = true;
        }
        if (v < -0.5f && prevV >= -0.5f)
        {
            y++;
            moved = true;
        }

        x = Mathf.Clamp(x, 0, 2);
        y = Mathf.Clamp(y, 0, 2);

        prevH = h;
        prevV = v;

        if (moved)
        {
            PlaySE(moveSE);
        }

        UpdateCursor();
    }

    void UpdateCursor()
    {
        StageButton target = buttons[x, y];

        if (target != null && cursor != null)
        {
            cursor.position = target.transform.position;
        }
    }

    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectStageCoroutine());
        }
    }

    void HandleBack()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            StartCoroutine(ReturnToModeSelectCoroutine());
        }
    }

    IEnumerator SelectStageCoroutine()
    {
        StageButton selected = buttons[x, y];
        if (selected == null) yield break;

        isTransitioning = true;

        PlaySE(decideSE);

        float wait = (decideSE != null) ? decideSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        GameSettings.selectedWorld = selected.world;
        GameSettings.selectedStage = selected.stage;

        SceneManager.LoadScene("演出");
    }

    IEnumerator ReturnToModeSelectCoroutine()
    {
        isTransitioning = true;

        PlaySE(backSE);

        float wait = (backSE != null) ? backSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene(modeSelectSceneName);
    }

    // =========================
    // 🔊 SE再生
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}