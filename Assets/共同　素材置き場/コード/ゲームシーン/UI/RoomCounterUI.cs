using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCounterUI : MonoBehaviour
{
    public Text roomText;

    private int remainingRooms;

    public void Init(int totalRooms)
    {
        remainingRooms = totalRooms;
        UpdateUI(false);
    }

    public void DecreaseRoom()
    {
        remainingRooms--;
        UpdateUI(true);
    }

    void UpdateUI(bool playAnimation)
    {
        roomText.text = "" + remainingRooms  ;

        if (playAnimation)
        {
            roomText.transform.localScale = Vector3.zero;

            roomText.transform
                .DOScale(1.3f, 0.2f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    roomText.transform.DOScale(1f, 0.1f);
                });
        }
    }
}