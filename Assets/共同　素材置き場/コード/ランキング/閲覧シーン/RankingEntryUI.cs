using UnityEngine;
using UnityEngine.UI;

public class RankingEntryUI : MonoBehaviour
{
    [Header("表示")]
    public Text rankText;
    public Text nameText;
    public Text scoreText;

    [Header("順位色")]
    public Color firstColor = new Color(1f, 0.84f, 0f);      // 金
    public Color secondColor = new Color(0.85f, 0.85f, 0.85f); // 銀
    public Color thirdColor = new Color(0.8f, 0.5f, 0.2f);     // 銅
    public Color normalColor = Color.white;

    public void SetData(
        int rank,
        string playerName,
        int score,
        bool isFirst,
        bool isSecond,
        bool isThird)
    {
        if (rankText != null)
            rankText.text = rank + "位";

        if (nameText != null)
            nameText.text = playerName;

        if (scoreText != null)
            scoreText.text = score.ToString();

        Color c = normalColor;

        if (isFirst)
            c = firstColor;
        else if (isSecond)
            c = secondColor;
        else if (isThird)
            c = thirdColor;

        if (rankText != null)
            rankText.color = c;

        if (nameText != null)
            nameText.color = c;

        if (scoreText != null)
            scoreText.color = c;
    }
}