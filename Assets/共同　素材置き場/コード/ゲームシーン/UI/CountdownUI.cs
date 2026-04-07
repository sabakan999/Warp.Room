using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class CountdownUI : MonoBehaviour
{
    public Image countdownImage;
    public Sprite[] numbers; // 0=3, 1=2, 2=1, 3=GO

    public float interval = 1f;

    public IEnumerator PlayCountdown()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < numbers.Length; i++)
        {
            countdownImage.sprite = numbers[i];

            // 初期状態
            countdownImage.transform.localScale = Vector3.zero;
            countdownImage.color = new Color(1, 1, 1, 1);

            // 🎯 ドン！演出
            Sequence seq = DOTween.Sequence();

            seq.Append(countdownImage.transform
                .DOScale(1.5f, 0.2f)
                .SetEase(Ease.OutBack));

            seq.Append(countdownImage.transform
                .DOScale(1f, 0.1f));

            // 少し待つ
            seq.AppendInterval(interval * 0.5f);

            // フェードアウト
            seq.Append(countdownImage
                .DOFade(0f, 0.2f));

            yield return seq.WaitForCompletion();
        }

        gameObject.SetActive(false);
    }
}