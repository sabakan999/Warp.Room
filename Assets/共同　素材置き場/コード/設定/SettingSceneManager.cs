using UnityEngine;

public class SettingSceneManager : MonoBehaviour
{
    [Header("設定項目")]
    public SettingItem[] items;

    private int currentIndex = 0;
    

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip saideSE;
    public AudioClip decideSE;

    float prevH = 0f;
    float prevV = 0f;


    void Start()
    {
        // 最初の項目を選択
        if (items.Length > 0)
        {
            items[currentIndex].OnSelected();
        }
    }

    void Update()
{
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");

    // ▼ 下移動
    if (Input.GetKeyDown(KeyCode.DownArrow) ||
        (v < -0.5f && prevV >= -0.5f))
    {
        MoveCursor(1);
    }

    // ▼ 上移動
    if (Input.GetKeyDown(KeyCode.UpArrow) ||
        (v > 0.5f && prevV <= 0.5f))
    {
        MoveCursor(-1);
    }

    // ▼ 左操作
    if (Input.GetKeyDown(KeyCode.LeftArrow) ||
        (h < -0.5f && prevH >= -0.5f))
    {
        items[currentIndex].OnLeft();
        PlaySE(saideSE);
    }

    // ▼ 右操作
    if (Input.GetKeyDown(KeyCode.RightArrow) ||
        (h > 0.5f && prevH <= 0.5f))
    {
        items[currentIndex].OnRight();
        PlaySE(saideSE);
    }

    // ▼ 決定（A）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        items[currentIndex].OnSubmit();
        PlaySE(decideSE);
    }

    

    

    prevH = h;
    prevV = v;
}


    void MoveCursor(int direction)
    {
        items[currentIndex].OnDeselected();

        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = items.Length - 1;

        if (currentIndex >= items.Length)
            currentIndex = 0;

        items[currentIndex].OnSelected();
        PlaySE(moveSE);
    }
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}