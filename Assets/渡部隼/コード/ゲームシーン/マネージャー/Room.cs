using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("プレイヤー出現位置")]
    public Transform playerSpawnPoint;

    public Transform GetSpawnPoint()
    {
        return playerSpawnPoint;
    }

    // 🔽 空でOK（エラー回避用）
    public void OnRoomStart()
    {
    }

    public void OnRoomEnd()
    {
    }
}