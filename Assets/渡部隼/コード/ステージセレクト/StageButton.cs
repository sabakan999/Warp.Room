using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public int world;
    public int stage;

    private Text text;
    private Vector3 defaultScale;

    void Start()
    {
        text = GetComponent<Text>();
        defaultScale = transform.localScale;
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale =
            isSelected ? defaultScale * 1.3f : defaultScale;
    }
}