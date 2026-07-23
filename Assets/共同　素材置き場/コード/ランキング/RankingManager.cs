using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    // 最大保存数
    public const int MaxRanking = 50;

    // ランキングデータ
    public List<RankingEntry> rankings = new List<RankingEntry>();

    // 保存先
    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "ranking.json"
            );
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadRanking();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //==========================
    // スコア追加
    //==========================
    public void AddScore(string playerName, int score)
    {
        rankings.Add(
            new RankingEntry(playerName, score)
        );

        rankings.Sort(
            (a, b) => b.score.CompareTo(a.score)
        );

        if (rankings.Count > MaxRanking)
        {
            rankings.RemoveRange(
                MaxRanking,
                rankings.Count - MaxRanking
            );
        }
        RankingAPI.Instance.PostScore(playerName, score);
        SaveRanking();
    }

    //==========================
    // 保存
    //==========================
    public void SaveRanking()
    {
        RankingData data = new RankingData();

        data.rankings = rankings;

        string json =
            JsonUtility.ToJson(data, true);

        File.WriteAllText(SavePath, json);
    }

    //==========================
    // 読み込み
    //==========================
    public void LoadRanking()
    {
        if (!File.Exists(SavePath))
        {
            rankings = new List<RankingEntry>();
            return;
        }

        string json =
            File.ReadAllText(SavePath);

        RankingData data =
            JsonUtility.FromJson<RankingData>(json);

        rankings = data.rankings;

        if (rankings == null)
            rankings = new List<RankingEntry>();
    }

    //==========================
    // 全削除（デバッグ用）
    //==========================
    public void ClearRanking()
    {
        rankings.Clear();

        SaveRanking();
    }
}