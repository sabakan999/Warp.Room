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

    [Header("参照")]
    public RoomManager roomManager;
    public PlayerSpawner playerSpawner;
    public TimerUI timerUI;
    public RoomCounterUI roomCounterUI;
    public CountdownUI countdownUI;

    [Header("デス演出")]
    public GameObject deathEffectPrefab; // ←一本化
    public ResultUI resultUI;

    [Header("プレイヤー状態")]
    public bool hasBarrier = false;

    private int currentCount = 0;
    private bool isGameRunning = false;

    public BGMManager bgmManager;

    void Start()
    {
        if (roomManager == null)
            roomManager = FindFirstObjectByType<RoomManager>();

        if (playerSpawner == null)
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();

        // =========================
        // モード分岐
        // =========================
        if (!GameSettings.isEndlessMode)
        {
            roomManager.currentLevel = GameSettings.selectedWorld;
            targetRoomCount = GameSettings.selectedStage * 3;

            if (roomCounterUI != null)
                roomCounterUI.Init(targetRoomCount);
        }
        else
        {
            // 🔥 エンドレスは全レベル出す
            roomManager.currentLevel = -1;
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

         bgmManager.PlayNormalBGM();

        isGameRunning = true;

        yield return StartCoroutine(SpawnAndPlayRoomLoop());

        // 🔥 エンドレスはクリアしない
        if (!GameSettings.isEndlessMode)
            GameClear();
    }

    IEnumerator SpawnAndPlayRoomLoop()
    {
        if (!GameSettings.isEndlessMode)
        {
            // 通常
            while (currentCount < targetRoomCount)
            {
                yield return StartCoroutine(PlayOneRoom());
                currentCount++;
            }
        }
        else
        {
            // エンドレス
            while (true)
            {
                yield return StartCoroutine(PlayOneRoom());
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

        room.GetComponent<Room>().OnRoomEnd();

        if (fadePanel != null)
            fadePanel.SetActive(true);

            playerSpawner.DespawnPlayer();

        yield return new WaitForSeconds(fadeDuration);

        roomManager.ClearCurrentRoom();
        playerSpawner.DespawnPlayer();

        if (fadePanel != null)
            fadePanel.SetActive(false);

        // 通常のみ減算
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
            resultUI.Show();
    }

    void GameClear()
    {
        isGameRunning = false;

        Debug.Log("CLEAR!");

        if (fadePanel != null)
            fadePanel.SetActive(true);

        hasBarrier = false;
    }
}