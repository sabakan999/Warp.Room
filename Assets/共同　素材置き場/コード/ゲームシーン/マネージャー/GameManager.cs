using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("ステージ設定")]
    public int targetRoomCount = 5;
    public float roomDuration = 3f;

    [Header("カウントダウン設定")]
    public float countdownInterval = 1f;

    [Header("暗転設定")]
    public GameObject fadePanel;
    public float fadeDuration = 0.5f;

    [Header("SE（ワープ）")]
    public AudioSource audioSource;
    public AudioClip warpStartSE;
    public AudioClip warpEndSE;

    [Header("参照")]
    public RoomManager roomManager;
    public PlayerSpawner playerSpawner;
    public TimerUI timerUI;
    public RoomCounterUI roomCounterUI;
    public CountdownUI countdownUI;

    [Header("デス演出")]
    public GameObject deathEffectPrefab;
    public ResultUI resultUI;

    [Header("プレイヤー状態")]
    public bool hasBarrier = false;

    private int currentCount = 0;
    private bool isGameRunning = false;
    private bool isClearing = false;

    // 🔥 無限用カウント
    private int endlessClearCount = 0;

    public BGMManager bgmManager;
    public ClearSequence clearSequence;

    void Start()
    {
        if (roomManager == null)
            roomManager = FindFirstObjectByType<RoomManager>();

        if (playerSpawner == null)
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();

        if (!GameSettings.isEndlessMode)
        {
            roomManager.currentLevel = GameSettings.selectedWorld;
            targetRoomCount = GameSettings.selectedStage * 3;
        }
        else
        {
            roomManager.currentLevel = -1;
            endlessClearCount = 0; // 初期化
        }

        if (roomCounterUI != null)
            roomCounterUI.Init(targetRoomCount);

        if (fadePanel != null)
            fadePanel.SetActive(false);

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        yield return StartCoroutine(Countdown());

        if (bgmManager != null)
            bgmManager.PlayNormalBGM();

        isGameRunning = true;

        yield return StartCoroutine(SpawnAndPlayRoomLoop());
    }

    IEnumerator SpawnAndPlayRoomLoop()
    {
        if (!GameSettings.isEndlessMode)
        {
            while (currentCount < targetRoomCount)
            {
                yield return StartCoroutine(PlayOneRoom());

                if (isClearing) yield break;

                currentCount++;
            }
        }
        else
        {
            while (true)
            {
                yield return StartCoroutine(PlayOneRoom());

                if (isClearing) yield break;

                // 🔥 無限はここでは増やさない（下で増やす）
                currentCount++;
            }
        }
    }

    IEnumerator PlayOneRoom()
    {
        GameObject room = roomManager.SpawnRoom();

        Transform spawnPoint = room.GetComponent<Room>().GetSpawnPoint();
        playerSpawner.SpawnPlayer(spawnPoint);

        room.GetComponent<Room>().OnRoomStart();

        if (timerUI != null)
            timerUI.StartTimer(roomDuration);

        yield return new WaitForSeconds(roomDuration);

        if (timerUI != null)
            timerUI.StopTimer();

        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();

        if (health != null && health.hasCurse)
         {
            health.TakeDamage();
            yield break;
         }

        // 🔥 ここが「クリア確定タイミング」
        if (GameSettings.isEndlessMode)
        {
            endlessClearCount++;
        }

        bool isLastRoom = (!GameSettings.isEndlessMode && currentCount + 1 >= targetRoomCount);

        if (isLastRoom)
        {
            GameClear();
            yield break;
        }

        // =========================
        // 🔥 ワープ処理
        // =========================
        room.GetComponent<Room>().OnRoomEnd();

        if (fadePanel != null)
        {
            PlaySE(warpStartSE);
            fadePanel.SetActive(true);
        }

        playerSpawner.DespawnPlayer();

        yield return new WaitForSeconds(fadeDuration);

        roomManager.ClearCurrentRoom();

        PlaySE(warpEndSE);

        if (fadePanel != null)
            fadePanel.SetActive(false);

        if (roomCounterUI != null)
            roomCounterUI.DecreaseRoom();
    }

    IEnumerator Countdown()
    {
        if (countdownUI != null)
        {
            yield return StartCoroutine(countdownUI.PlayCountdown());
        }
        else
        {
            int count = 3;

            while (count > 0)
            {
                Debug.Log(count);
                yield return new WaitForSeconds(countdownInterval);
                count--;
            }

            Debug.Log("START!");
        }
    }

    public void GameOver()
    {
        if (!isGameRunning) return;

        isGameRunning = false;

        StopAllCoroutines();

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        Debug.Log("GAME OVER");

        if (timerUI != null)
            timerUI.StopTimer();

        if (bgmManager != null)
            bgmManager.StopBGM();

        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, player.transform.position, Quaternion.identity);

            Destroy(player.gameObject);
        }

        hasBarrier = false;

        yield return new WaitForSeconds(0.8f);

        if (resultUI != null)
        {
            if (GameSettings.isEndlessMode)
                resultUI.SetEndlessResult(endlessClearCount);

            resultUI.Show();
        }
    }

    void GameClear()
    {
        if (isClearing) return;

        isClearing = true;
        isGameRunning = false;

        StopAllCoroutines();

        if (timerUI != null)
            timerUI.StopTimer();

        if (bgmManager != null)
            bgmManager.StopBGM();

        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (clearSequence != null && player != null)
        {
            clearSequence.Play(player.transform);
        }

        hasBarrier = false;
    }

    // =========================
    // 🔥 外部取得用
    // =========================
    public int GetEndlessClearCount()
    {
        return endlessClearCount;
    }

    // =========================
    // 🔊 SE
    // =========================
    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}