using UnityEngine;

public class UIRotate : MonoBehaviour
{
    [Header("回転速度")]
    [SerializeField] private float speed = 100f;

    [Header("時計回りならON")]
    [SerializeField] private bool clockwise = true;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float direction = clockwise ? -1f : 1f;

        rectTransform.Rotate(
            0f,
            0f,
            speed * direction * Time.deltaTime
        );
    }
}