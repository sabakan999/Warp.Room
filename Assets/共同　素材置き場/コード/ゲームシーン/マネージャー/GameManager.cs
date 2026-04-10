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
    public GameObject deathEffectPrefab;
    public ResultUI resultUI;

    private int currentCount = 0;
    private bool isGameRunning = false;

    void Start()
    {
        if (roomManager == null)
            roomManager = FindFirstObjectByType<RoomManager>();

        if (playerSpawner == null)
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();

        roomManager.currentLevel = GameSettings.selectedWorld;
        targetRoomCount = GameSettings.selectedStage * 3;

        if (fadePanel != null)
            fadePanel.SetActive(false);

        roomCounterUI.Init(targetRoomCount);

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        yield return StartCoroutine(Countdown());

        isGameRunning = true;

        yield return StartCoroutine(SpawnAndPlayRoomLoop());

        GameClear();
    }

    IEnumerator SpawnAndPlayRoomLoop()
    {
        while (currentCount < targetRoomCount)
        {
            GameObject room = roomManager.SpawnRoom();

            Transform spawnPoint = room.GetComponent<Room>().GetSpawnPoint();
            playerSpawner.SpawnPlayer(spawnPoint);

            room.GetComponent<Room>().OnRoomStart();

            // 🎯 タイマー開始
            if (timerUI != null)
                timerUI.StartTimer(roomDuration);

            yield return new WaitForSeconds(roomDuration);

            // 🎯 タイマー停止
            if (timerUI != null)
                timerUI.StopTimer();

            room.GetComponent<Room>().OnRoomEnd();

            if (fadePanel != null)
                fadePanel.SetActive(true);

            yield return new WaitForSeconds(fadeDuration);

            roomManager.ClearCurrentRoom();
            playerSpawner.DespawnPlayer();

            if (fadePanel != null)
                fadePanel.SetActive(false);

            if (roomCounterUI != null)
                roomCounterUI.DecreaseRoom();

            currentCount++;
        }
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

    // 🔥 即爆散版
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

        // 🎯 タイマー止める
        if (timerUI != null)
            timerUI.StopTimer();

        PlayerController player = FindFirstObjectByType<PlayerController>();

        // 💥 即爆散
        if (player != null)
        {
            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, player.transform.position, Quaternion.identity);

            Destroy(player.gameObject);
        }

        // 🔥 少し余韻（ここだけ通常時間）
        yield return new WaitForSeconds(0.8f);

        // 🎯 リザルト表示
        if (resultUI != null)
            resultUI.Show();
    }

    void GameClear()
    {
        isGameRunning = false;

        Debug.Log("CLEAR!");

        if (fadePanel != null)
            fadePanel.SetActive(true);
    }
}