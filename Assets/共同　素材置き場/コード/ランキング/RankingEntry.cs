using System;

[Serializable]
public class RankingEntry
{
    public string playerName;
    public int score;

    public RankingEntry(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}