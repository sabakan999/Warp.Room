using UnityEngine;
using UnityEngine.UI;

public class SettingItem : MonoBehaviour
{
    [Header("選択表示")]
    public Text label;

    public virtual void OnSelected()
    {
        if (label != null)
            label.color = new Color(1.0f, 0.55f, 0.0f);   // オレンジ
    }

    public virtual void OnDeselected()
    {
        if (label != null)
            label.color = Color.black;   // 黒
    }

    public virtual void OnLeft() { }

    public virtual void OnRight() { }

    public virtual void OnSubmit() { }
}