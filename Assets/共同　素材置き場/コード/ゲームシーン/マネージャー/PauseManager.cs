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

    if (Input.GetKeyDown(KeyCode.Escape))
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

        //---------------------------------
        // パネル決定
        //---------------------------------
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
    // カーソル移動
    //---------------------------------
    void HandleMove()
    {
        int old = currentIndex;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = currentItems.Length - 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex++;

            if (currentIndex >= currentItems.Length)
                currentIndex = 0;
        }

        if (old != currentIndex)
        {
            PlaySE(moveSE);

            cursor.position =
                currentItems[currentIndex].position;
        }
    }

    //---------------------------------
    // 決定
    //---------------------------------
    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
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

        //---------------------------------
        // チュートリアル
        //---------------------------------
        if (isTutorial)
        {
            switch (currentIndex)
            {
                // 続ける
                case 0:
                    Resume();
                    break;

                // チュートリアル終了
                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("モードセレクト");
                    break;
            }
        }

        //---------------------------------
        // エンドレス
        //---------------------------------
        else if (GameSettings.isEndlessMode)
        {
            switch (currentIndex)
            {
                // 続ける
                case 0:
                    Resume();
                    break;

                // リトライ
                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(
                        SceneManager.GetActiveScene().name
                    );
                    break;

                // モードセレクト
                case 2:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("モードセレクト");
                    break;
            }
        }

        //---------------------------------
        // ノーマル
        //---------------------------------
        else
        {
            switch (currentIndex)
            {
                // 続ける
                case 0:
                    Resume();
                    break;

                // リトライ
                case 1:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(
                        SceneManager.GetActiveScene().name
                    );
                    break;

                // ステージセレクト
                case 2:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("ステージセレクト");
                    break;

                // モードセレクト
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