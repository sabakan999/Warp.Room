using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("モード")]
    public bool isTutorial = false;

    [Header("パネル")]
    public GameObject tutorialPanel;
    public GameObject normalPanel;
    public GameObject endlessPanel;

    [Header("カーソル")]
    public RectTransform cursor;

    [Header("Tutorial")]
    public RectTransform[] tutorialItems;

    [Header("Normal")]
    public RectTransform[] normalItems;

    [Header("Endless")]
    public RectTransform[] endlessItems;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip moveSE;
    public AudioClip decideSE;

    private bool isPaused = false;
    private bool isDeciding = false;

    private int currentIndex = 0;

    private RectTransform[] currentItems;
    private GameObject currentPanel;
    public bool canPause = true;

    // --- スティック長押し防止用 ---
    bool stickUpPrev = false;
    bool stickDownPrev = false;

    void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (normalPanel != null)
            normalPanel.SetActive(false);

        if (endlessPanel != null)
            endlessPanel.SetActive(false);

        if (cursor != null)
            cursor.gameObject.SetActive(false);

        canPause = true;
    }

    void Update()
    {
        if (!canPause)
            return;

        if (isDeciding)
            return;

        // --- Pause / Resume ---
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetButtonDown("PauseButton")) 
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }

        if (!isPaused)
            return;

        HandleMove();
        HandleSubmit();
    }

    //---------------------------------
    // ポーズ開始
    //---------------------------------
    void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;

        currentIndex = 0;

        if (isTutorial)
        {
            currentPanel = tutorialPanel;
            currentItems = tutorialItems;
        }
        else if (GameSettings.isEndlessMode)
        {
            currentPanel = endlessPanel;
            currentItems = endlessItems;
        }
        else
        {
            currentPanel = normalPanel;
            currentItems = normalItems;
        }

        if (currentPanel != null)
            currentPanel.SetActive(true);

        if (cursor != null &&
            currentItems != null &&
            currentItems.Length > 0)
        {
            cursor.gameObject.SetActive(true);
            cursor.position = currentItems[currentIndex].position;
        }
    }

    //---------------------------------
    // 再開
    //---------------------------------
    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (currentPanel != null)
            currentPanel.SetActive(false);

        cursor.gameObject.SetActive(false);
    }

    //---------------------------------
    // カーソル移動（長押し禁止 + 高レスポンス）
    //---------------------------------
    void HandleMove()
    {
        int old = currentIndex;

        // --- キーボード ---
        bool upKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKey = Input.GetKeyDown(KeyCode.DownArrow);

        // --- コントローラー（左スティック） ---
        float v = Input.GetAxis("Vertical");

        bool stickUpNow = v > 0.5f;
        bool stickDownNow = v < -0.5f;

        // 「前フレームは入力なし → 今フレーム入力あり」だけ反応
        bool upJoy = !stickUpPrev && stickUpNow;
        bool downJoy = !stickDownPrev && stickDownNow;

        // --- 上移動 ---
        if (upKey || upJoy)
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = currentItems.Length - 1;
        }

        // --- 下移動 ---
        if (downKey || downJoy)
        {
            currentIndex++;
            if (currentIndex >= currentItems.Length)
                currentIndex = 0;
        }

        // --- カーソル更新 ---
        if (old != currentIndex)
        {
            PlaySE(moveSE);
            cursor.position = currentItems[currentIndex].position;
        }

        // 入力状態を記録
        stickUpPrev = stickUpNow;
        stickDownPrev = stickDownNow;
    }

    //---------------------------------
    // 決定（キーボード + Aボタン）
    //---------------------------------
    void HandleSubmit()
    {
        bool keySubmit =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return);

        bool joySubmit = Input.GetButtonDown("Submit"); // Aボタン

        if (keySubmit || joySubmit)
        {
            StartCoroutine(DecideRoutine());
        }
    }

    //---------------------------------
    // 決定処理
    //---------------------------------
    System.Collections.IEnumerator DecideRoutine()
    {
        isDeciding = true;

        PlaySE(decideSE);

        float wait = (decideSE != null)
            ? decideSE.length
            : 0.2f;

        yield return new WaitForSecondsRealtime(wait);

        if (isTutorial)
        {
            switch (currentIndex)
            {
                case 0:
                    Resume();
                    break;

                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("モードセレクト");
                    break;
            }
        }
        else if (GameSettings.isEndlessMode)
        {
            switch (currentIndex)
            {
                case 0:
                    Resume();
                    break;

                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(
                        SceneManager.GetActiveScene().name
                    );
                    break;

                case 2:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("モードセレクト");
                    break;
            }
        }
        else
        {
            switch (currentIndex)
            {
                case 0:
                    Resume();
                    break;

                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(
                        SceneManager.GetActiveScene().name
                    );
                    break;

                case 2:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("ステージセレクト");
                    break;

                case 3:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("モードセレクト");
                    break;
            }
        }

        isDeciding = false;
    }

    //---------------------------------
    // SE
    //---------------------------------
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}
