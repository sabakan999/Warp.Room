using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class RankingAPI : MonoBehaviour
{
    public static RankingAPI Instance;

    public List<RankingEntry> rankings = new List<RankingEntry>();

    [Header("PHP URL")]
    public string postURL =
        "http://2025isc1240103.watson.jp/upload_score.php";

    public string getURL =
        "http://2025isc1240103.watson.jp/get_ranking.php";

    public string deleteURL =
        "http://2025isc1240103.watson.jp/delete_score.php";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //====================================
    // スコア送信
    //====================================
    public void PostScore(string playerName, int score, System.Action onComplete)
    {
        StartCoroutine(PostRoutine(playerName, score, onComplete));
    }

    IEnumerator PostRoutine(string playerName, int score, System.Action onComplete)
    {
        WWWForm form = new WWWForm();

        form.AddField("name", playerName);
        form.AddField("score", score);

        UnityWebRequest request =
            UnityWebRequest.Post(postURL, form);

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("送信成功");
        }

        onComplete?.Invoke();
    }

    //====================================
    // スコア削除
    //====================================
 
    public void DeleteScore(string playerName, int score, System.Action onComplete)
    {
        StartCoroutine(DeleteRoutine(playerName, score, onComplete));
    }

    IEnumerator DeleteRoutine(string playerName, int score, System.Action onComplete)
    {
        WWWForm form = new WWWForm();

        form.AddField("name", playerName);
        form.AddField("score", score);

        UnityWebRequest request =
            UnityWebRequest.Post(deleteURL, form);

        yield return request.SendWebRequest();

    #if UNITY_2020_2_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
    #else
        if (request.isNetworkError || request.isHttpError)
    #endif
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("削除成功");
            Debug.Log(request.downloadHandler.text);
        }

        onComplete?.Invoke();
    }

    //====================================
    // ランキング取得
    //====================================
    public void GetRanking(System.Action onComplete)
    {
        StartCoroutine(GetRoutine(onComplete));
    }

    IEnumerator GetRoutine(System.Action onComplete)
    {
        UnityWebRequest request =
            UnityWebRequest.Get(getURL);

        yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;

            RankingEntryArray data =
                JsonUtility.FromJson<RankingEntryArray>(
                    "{\"rankings\":" + json + "}"
                );

            RankingManager.Instance.rankings =
                new List<RankingEntry>(data.rankings);

            rankings =
                new List<RankingEntry>(data.rankings);

            Debug.Log("ランキング取得完了");
        }

        onComplete?.Invoke();
    }
}

[System.Serializable]
public class RankingEntryArray
{
    public RankingEntry[] rankings;
}