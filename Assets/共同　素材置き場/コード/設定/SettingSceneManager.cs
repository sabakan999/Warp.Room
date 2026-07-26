using UnityEngine;

public class SettingSceneManager : MonoBehaviour
{
    [Header("設定項目")]
    public SettingItem[] items;

    private int currentIndex = 0;

    void Start()
    {
        // 最初の項目を選択
        if (items.Length > 0)
        {
            items[currentIndex].OnSelected();
        }
    }

    void Update()
    {
        // 下
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(1);
        }

        // 上
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(-1);
        }

        // 左
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            items[currentIndex].OnLeft();
        }

        // 右
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            items[currentIndex].OnRight();
        }

        // 決定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            items[currentIndex].OnSubmit();
        }
    }

    void MoveCursor(int direction)
    {
        items[currentIndex].OnDeselected();

        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = items.Length - 1;

        if (currentIndex >= items.Length)
            currentIndex = 0;

        items[currentIndex].OnSelected();
    }
}