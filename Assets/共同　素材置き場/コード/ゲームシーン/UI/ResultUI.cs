using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }
}