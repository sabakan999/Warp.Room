using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public StageButton[,] buttons = new StageButton[3, 3];

    public RectTransform cursor; // ←追加

    private int x = 0;
    private int y = 0;

    float prevH = 0f;
    float prevV = 0f;

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
        HandleMove();
        HandleSubmit();
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h > 0.5f && prevH <= 0.5f) x++;
        if (h < -0.5f && prevH >= -0.5f) x--;

        if (v > 0.5f && prevV <= 0.5f) y--;
        if (v < -0.5f && prevV >= -0.5f) y++;

        x = Mathf.Clamp(x, 0, 2);
        y = Mathf.Clamp(y, 0, 2);

        prevH = h;
        prevV = v;

        UpdateCursor();
    }

    void UpdateCursor()
    {
        StageButton target = buttons[x, y];

        if (target != null && cursor != null)
        {
            // 位置を一致させる
            cursor.position = target.transform.position;
        }
    }

    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            SelectStage();
        }
    }

    void SelectStage()
    {
        StageButton selected = buttons[x, y];

        if (selected == null) return;

        GameSettings.selectedWorld = selected.world;
        GameSettings.selectedStage = selected.stage;

        SceneManager.LoadScene("ワープ・ルーム");
    }
}