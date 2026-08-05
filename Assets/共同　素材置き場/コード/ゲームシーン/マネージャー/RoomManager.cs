using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("スポーン位置")]
    public Transform spawnPoint;

    [Header("レベル別部屋")]
    public List<GameObject> level1Rooms;
    public List<GameObject> level2Rooms;
    public List<GameObject> level3Rooms;

    [Header("テスト用")]
    public List<GameObject> testRooms;
    public bool useTestRooms = true;

    [Header("現在のレベル")]
    public int currentLevel = 1;

    public GameObject bossRoomPrefab;

    private bool usedTestRoom = false;
    private GameObject currentRoom;

    private GameObject lastRoomPrefab = null;

    // 🔥 進行度
    private int progress = 0;

    // =========================
    public void SetProgress(int value)
    {
        progress = value;
    }

    // =========================
    public GameObject SpawnRoom()
    {
        GameObject roomPrefab = null;

        // テスト部屋
        if (useTestRooms && !usedTestRoom && testRooms != null && testRooms.Count > 0)
        {
            roomPrefab = testRooms[Random.Range(0, testRooms.Count)];
            usedTestRoom = true;
        }
        else
        {
            roomPrefab = GetRoomByMode();
        }

        currentRoom = Instantiate(roomPrefab, spawnPoint.position, Quaternion.identity);
        return currentRoom;
    }

    // =========================
    GameObject GetRoomByMode()
    {
        // 🔥 無限モード
        if (currentLevel == -1)
        {
            return GetWeightedRandomRoom();
        }

        // 通常
        var list = GetRoomListByLevel();
        return GetRandomRoomAvoidRepeat(list);
    }

    // =========================
    // 🔥 重み付き抽選（ここが本体）
    // =========================
    GameObject GetWeightedRandomRoom()
    {
        float w1 = Mathf.Max(1f, 10f - progress * 0.5f);
        float w2 = Mathf.Clamp(2f + progress * 0.3f, 2f, 10f);
        float w3 = Mathf.Clamp(progress * 0.5f, 1f, 15f);

        float total = w1 + w2 + w3;
        float rand = Random.Range(0f, total);

        List<GameObject> selectedList;

        if (rand < w1)
            selectedList = level1Rooms;
        else if (rand < w1 + w2)
            selectedList = level2Rooms;
        else
            selectedList = level3Rooms;

        return GetRandomRoomAvoidRepeat(selectedList);
    }

    // =========================
    GameObject GetRandomRoomAvoidRepeat(List<GameObject> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("部屋リスト空！");
            return null;
        }

        if (list.Count == 1)
            return list[0];

        GameObject selected = null;
        int safety = 0;

        do
        {
            selected = list[Random.Range(0, list.Count)];
            safety++;
        }
        while (selected == lastRoomPrefab && safety < 10);

        lastRoomPrefab = selected;

        return selected;
    }

    // =========================
    List<GameObject> GetRoomListByLevel()
    {
        switch (currentLevel)
        {
            case 1: return level1Rooms;
            case 2: return level2Rooms;
            case 3: return level3Rooms;
            default: return level1Rooms;
        }
    }

    // =========================
    public void ClearCurrentRoom()
    {
        if (MultiSEManager.Instance != null)
        {
          MultiSEManager.Instance.StopAllSE();
        }
        if (currentRoom != null)
            Destroy(currentRoom);
    }

        public GameObject SpawnBossRoom()
    {
        currentRoom = Instantiate(
            bossRoomPrefab,
            spawnPoint.position,
            Quaternion.identity);

        return currentRoom;
    }

    public GameObject CurrentRoom
{
    get { return currentRoom; }
}
}