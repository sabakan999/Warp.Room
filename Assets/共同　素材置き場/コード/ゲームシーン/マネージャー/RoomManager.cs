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

    [Header("テスト用（最初に優先）")]
    public List<GameObject> testRooms;
    public bool useTestRooms = true;

    [Header("現在のレベル")]
    public int currentLevel = 1;

    private bool usedTestRoom = false;
    private GameObject currentRoom;

    // =========================
    // 部屋生成（GameManagerから呼ばれる）
    // =========================
    public GameObject SpawnRoom()
    {
        GameObject roomPrefab = null;

        // 🔥 テスト部屋優先
        if (useTestRooms && !usedTestRoom && testRooms != null && testRooms.Count > 0)
        {
            int index = Random.Range(0, testRooms.Count);
            roomPrefab = testRooms[index];

            usedTestRoom = true;
        }
        else
        {
            List<GameObject> targetList = GetRoomListByLevel();

            if (targetList == null || targetList.Count == 0)
            {
                Debug.LogError("部屋リストが空！");
                return null;
            }

            int index = Random.Range(0, targetList.Count);
            roomPrefab = targetList[index];
        }

        currentRoom = Instantiate(roomPrefab, spawnPoint.position, Quaternion.identity);

        return currentRoom;
    }

    // =========================
    // レベルに応じたリスト取得
    // =========================
    List<GameObject> GetRoomListByLevel()
    {
        switch (currentLevel)
        {
            case 1:
                return level1Rooms;
            case 2:
                return level2Rooms;
            case 3:
                return level3Rooms;
            default:
                return level1Rooms;
        }
    }

    // =========================
    // 現在の部屋削除
    // =========================
    public void ClearCurrentRoom()
    {
        if (currentRoom != null)
        {
            Destroy(currentRoom);
        }
    }
}