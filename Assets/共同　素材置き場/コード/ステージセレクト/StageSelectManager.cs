using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
    public StageButton[,] buttons = new StageButton[3, 3];

    public RectTransform cursor;

    [Header("カーソル")]
    public float cursorMoveSpeed = 15f;
    public float blinkSpeed = 2f;
    public float selectBlinkSpeed = 12f;

   

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;
    public AudioClip falseSE;

    [Header("隠しコマンド")]
    public float resetHoldTime = 10f;
    public int unlockTapCount = 30;

    private float escapeHoldTimer = 0f;
    private int escapeTapCount = 0;
    private float escapeTapResetTime = 2f;
    private float lastEscapeTapTime = 0f;
    

    private int x = 0;
    private int y = 0;

    float prevH = 0f;
    float prevV = 0f;

    bool isTransitioning = false;

    private Vector3 targetCursorPosition;
    private Image cursorImage;

    void Start()
    {
        GameSettings.Load();

    StageButton[] stageButtons = GetComponentsInChildren<StageButton>();
       

        int index = 0;
        foreach (StageButton btn in stageButtons)
        {
             btn.UpdateLock();
            int row = index / 3;
            int col = index % 3;

            if (col < 3 && row < 3)
                buttons[row, col] = btn;

            index++;
        }

        cursorImage = cursor.GetComponent<Image>();

        UpdateCursor();

        if (cursor != null)
            targetCursorPosition = cursor.position;
    }

    void Update()
    {
        if (!isTransitioning)
        {
            HandleMove();
            HandleSubmit();

             HandleSecretCommand();
            
        }

        UpdateCursorMove();
        UpdateCursorBlink();
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
            PlaySE(moveSE);

        UpdateCursor();
    }


    // =========================
// 🔥 隠しコマンド
// =========================
void HandleSecretCommand()
{
    // ESC長押し ＋ PauseButton長押し
    if (Input.GetKey(KeyCode.Escape) ||
        Input.GetButton("PauseButton"))   // ← 追加
    {
        escapeHoldTimer += Time.deltaTime;

        if (escapeHoldTimer >= resetHoldTime)
        {
            ResetProgress();
            escapeHoldTimer = 0f;
        }
    }
    else
    {
        escapeHoldTimer = 0f;
    }

    // ESC連打 ＋ PauseButton連打
    if (Input.GetKeyDown(KeyCode.Escape) ||
        Input.GetButtonDown("PauseButton"))   // ← 追加
    {
        if (Time.time - lastEscapeTapTime < escapeTapResetTime)
        {
            escapeTapCount++;
        }
        else
        {
            escapeTapCount = 1;
        }

        lastEscapeTapTime = Time.time;

        if (escapeTapCount >= unlockTapCount)
        {
            UnlockAllStages();
            escapeTapCount = 0;
        }
    }
}

    void UpdateCursor()
    {
        StageButton target = buttons[x, y];

        if (target != null)
            targetCursorPosition = target.transform.position;
    }

    void UpdateCursorMove()
    {
        if (cursor == null) return;

        cursor.position = Vector3.Lerp(
            cursor.position,
            targetCursorPosition,
            cursorMoveSpeed * Time.deltaTime
        );
    }

    void UpdateCursorBlink()
    {
        if (cursorImage == null)
            return;

        Color c = cursorImage.color;

        if (isTransitioning)
        {
            // 決定時：100%⇔0%
            c.a = Mathf.Lerp(
                0f,
                1f,
                (Mathf.Sin(Time.time * selectBlinkSpeed * Mathf.PI) + 1f) * 0.5f
            );
        }
        else
        {
            // 通常時：100%⇔40%
            c.a = Mathf.Lerp(
                0.4f,
                1f,
                (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) * 0.5f
            );
        }

        cursorImage.color = c;
    }

   void HandleSubmit()
{
    // Aボタン（決定）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        StartCoroutine(SelectStageCoroutine());
        return;
    }

   

    
}

    

   

    IEnumerator SelectStageCoroutine()
    {
        StageButton selected = buttons[x, y];
        if (selected == null)
            yield break;
         
         if (!selected.IsUnlocked())
         {
            PlaySE(falseSE);
            yield break;
         }
        

        isTransitioning = true;

        PlaySE(decideSE);

        float wait = (decideSE != null) ? decideSE.length : 0.2f;

        yield return new WaitForSeconds(wait);

        GameSettings.selectedWorld = selected.world;
        GameSettings.selectedStage = selected.stage;

        SceneManager.LoadScene("演出");
    }

    void ResetProgress()
{
    PlayerPrefs.DeleteKey("UnlockedWorld");
    PlayerPrefs.DeleteKey("UnlockedStage");

    GameSettings.unlockedWorld = 1;
    GameSettings.unlockedStage = 1;

    Debug.Log("進行データ初期化");
     SceneManager.LoadScene(
        SceneManager.GetActiveScene().name
    );
}

void UnlockAllStages()
{
    GameSettings.unlockedWorld = 99;
    GameSettings.unlockedStage = 99;


    PlayerPrefs.SetInt(
        "UnlockedWorld",
        GameSettings.unlockedWorld
    );

    PlayerPrefs.SetInt(
        "UnlockedStage",
        GameSettings.unlockedStage
    );


    PlayerPrefs.Save();
     
     SceneManager.LoadScene(
        SceneManager.GetActiveScene().name
    );


    Debug.Log("全ステージ解放");
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