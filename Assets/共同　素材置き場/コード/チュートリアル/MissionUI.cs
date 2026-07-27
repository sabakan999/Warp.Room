using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public GameObject root;
    public Text missionText;

    public void Show(string text)
    {
        root.SetActive(true);
        missionText.text = text;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}