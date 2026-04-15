using UnityEngine;

public class ConveyorMaskedScroll : MonoBehaviour
{
    [Header("流す矢印帯")]
    public Transform[] strips;

    [Header("スクロール速度")]
    public float scrollSpeed = 2f;

    [Header("方向")]
    public Vector2 scrollDirection = Vector2.left;

    [Header("帯1枚の幅")]
    public float stripWidth = 2f;

    void Update()
    {
        if (strips == null || strips.Length == 0)
            return;

        Vector3 move = (Vector3)(scrollDirection.normalized * scrollSpeed * Time.deltaTime);

        // 全部動かす
        foreach (Transform strip in strips)
        {
            strip.localPosition += move;
        }

        // 左流れ
        if (scrollDirection.x < 0)
        {
            float rightMostX = GetRightMostX();

            foreach (Transform strip in strips)
            {
                if (strip.localPosition.x <= rightMostX - stripWidth * strips.Length)
                {
                    strip.localPosition = new Vector3(
                        rightMostX + stripWidth,
                        strip.localPosition.y,
                        strip.localPosition.z
                    );

                    rightMostX = strip.localPosition.x;
                }
            }
        }

        // 右流れ
        if (scrollDirection.x > 0)
        {
            float leftMostX = GetLeftMostX();

            foreach (Transform strip in strips)
            {
                if (strip.localPosition.x >= leftMostX + stripWidth * strips.Length)
                {
                    strip.localPosition = new Vector3(
                        leftMostX - stripWidth,
                        strip.localPosition.y,
                        strip.localPosition.z
                    );

                    leftMostX = strip.localPosition.x;
                }
            }
        }
    }

    float GetRightMostX()
    {
        float max = strips[0].localPosition.x;

        foreach (Transform strip in strips)
        {
            if (strip.localPosition.x > max)
                max = strip.localPosition.x;
        }

        return max;
    }

    float GetLeftMostX()
    {
        float min = strips[0].localPosition.x;

        foreach (Transform strip in strips)
        {
            if (strip.localPosition.x < min)
                min = strip.localPosition.x;
        }

        return min;
    }
}