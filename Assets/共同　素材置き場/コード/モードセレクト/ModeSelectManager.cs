using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ModeSelectManager : MonoBehaviour
{
    [Header("選択肢UI")]
    public RectTransform normalButton;
    public RectTransform endlessButton;

    [Header("拡大設定")]
    public float selectedScale = 1.2f;
    public float normalScale = 1.0f;
    public float scaleTime = 0.15f;

    [Header("次シーン")]
    public string normalNextScene = "ステージセレクト";
    public string endlessNextScene = "ワープ・ルーム";

    private int selectedIndex = 0; // 0=ノーマル 1=エンドレス
    private float prevV = 0f;

    void Start()
    {
        UpdateSelection(true);
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

        UpdateSelection(false);
    }

    void UpdateSelection(bool instant)
    {
        if (normalButton == null || endlessButton == null)
            return;

        float nScale = (selectedIndex == 0) ? selectedScale : normalScale;
        float eScale = (selectedIndex == 1) ? selectedScale : normalScale;

        if (instant)
        {
            normalButton.localScale = Vector3.one * nScale;
            endlessButton.localScale = Vector3.one * eScale;
        }
        else
        {
            // 🔥 既存Tweenを消す（超重要）
            normalButton.DOKill();
            endlessButton.DOKill();

            // 🔥 新しくTween（リンク付き）
            normalButton
                .DOScale(nScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(normalButton.gameObject);

            endlessButton
                .DOScale(eScale, scaleTime)
                .SetEase(Ease.OutBack)
                .SetLink(endlessButton.gameObject);
        }
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
        // 🔥 全Tween停止（シーン遷移対策）
        DOTween.KillAll();

        if (selectedIndex == 0)
        {
            GameSettings.isEndlessMode = false;
            SceneManager.LoadScene(normalNextScene);
        }
        else
        {
            GameSettings.isEndlessMode = true;
            SceneManager.LoadScene(endlessNextScene);
        }
    }
}