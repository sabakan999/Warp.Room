using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectManager : MonoBehaviour
{
    [Header("カーソル")]
    public RectTransform cursor;

    [Header("選択肢UI")]
    public RectTransform normalButton;
    public RectTransform endlessButton;

    [Header("次シーン")]
    public string normalNextScene = "ステージセレクト";
    public string endlessNextScene = "ワープ・ルーム";

    private int selectedIndex = 0; // 0=ノーマル 1=エンドレス

    private float prevV = 0f;

    void Start()
    {
        UpdateCursor();
    }

    void Update()
    {
        HandleMove();
        HandleSubmit();
    }

    void HandleMove()
    {
        float v = Input.GetAxisRaw("Vertical");

        if (v > 0.5f && prevV <= 0.5f)
        {
            selectedIndex--;
        }
        else if (v < -0.5f && prevV >= -0.5f)
        {
            selectedIndex++;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, 1);

        prevV = v;

        UpdateCursor();
    }

    void UpdateCursor()
    {
        if (cursor == null) return;

        if (selectedIndex == 0)
            cursor.position = normalButton.position;
        else
            cursor.position = endlessButton.position;
    }

    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            SelectMode();
        }
    }

    void SelectMode()
    {
        if (selectedIndex == 0)
        {
            // ノーマル
            GameSettings.isEndlessMode = false;
            SceneManager.LoadScene(normalNextScene);
        }
        else
        {
            // エンドレス
            GameSettings.isEndlessMode = true;

            // エンドレスは仮でそのままゲームへ
            SceneManager.LoadScene(endlessNextScene);
        }
    }
}