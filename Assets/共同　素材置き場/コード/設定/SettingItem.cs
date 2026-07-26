using UnityEngine;
using UnityEngine.UI;

public class SettingItem : MonoBehaviour
{
    [Header("選択表示")]
    public Text label;

    public virtual void OnSelected()
    {
        if (label != null)
            label.color = Color.yellow;
    }

    public virtual void OnDeselected()
    {
        if (label != null)
            label.color = Color.white;
    }

    public virtual void OnLeft() { }

    public virtual void OnRight() { }

    public virtual void OnSubmit() { }
}