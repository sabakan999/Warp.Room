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
    public GameObject fadePanel; // 黒パネル
    public float fadeDuration = 0.5f;

    [Header("参照")]
    public RoomManager roomManager;
    public PlayerSpawner playerSpawner;

    private int currentCount = 0;
    private bool isGameRunning = false;


    void Start()
    {
        if (roomManager == null)
            roomManager = FindFirstObjectByType<RoomManager>();

        if (playerSpawner == null)
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();
  
    roomManager.currentLevel = GameSettings.selectedWorld;
    targetRoomCount = GameSettings.selectedStage * 3; // 例：1-3なら3部屋

        // 最初は暗転オフ
        if (fadePanel != null)
            fadePanel.SetActive(false);

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
            // 🔥 部屋生成
            GameObject room = roomManager.SpawnRoom();

            Transform spawnPoint = room.GetComponent<Room>().GetSpawnPoint();
            playerSpawner.SpawnPlayer(spawnPoint);

            room.GetComponent<Room>().OnRoomStart();

            // 🔥 3秒生存
            yield return new WaitForSeconds(roomDuration);

            room.GetComponent<Room>().OnRoomEnd();

            // 🔥 暗転開始（ここ重要）
            if (fadePanel != null)
                fadePanel.SetActive(true);

            // 🔥 0.5秒待つ（まだ部屋は残ってる）
            yield return new WaitForSeconds(fadeDuration);

            // 🔥 部屋削除（暗転中）
            roomManager.ClearCurrentRoom();

            // 🔥 プレイヤーも消す（任意だけどおすすめ）
            playerSpawner.DespawnPlayer();

            // 🔥 暗転解除
            if (fadePanel != null)
                fadePanel.SetActive(false);

            currentCount++;
        }
    }

    IEnumerator Countdown()
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

    public void GameOver()
    {
        if (!isGameRunning) return;

        isGameRunning = false;

        StopAllCoroutines();

        Debug.Log("GAME OVER");

        if (fadePanel != null)
            fadePanel.SetActive(true);

        if (roomManager != null)
            roomManager.ClearCurrentRoom();

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.DisableControl();
    }

    void GameClear()
    {
        isGameRunning = false;

        Debug.Log("CLEAR!");

        if (fadePanel != null)
            fadePanel.SetActive(true);
    }
  
}