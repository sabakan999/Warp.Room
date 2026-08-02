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
    

    private int x = 0;
    private int y = 0;

    float prevH = 0f;
    float prevV = 0f;

    bool isTransitioning = false;

    private Vector3 targetCursorPosition;
    private Image cursorImage;

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
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectStageCoroutine());
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

   

    // =========================
    // 🔊 SE再生
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}