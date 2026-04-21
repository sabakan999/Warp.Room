using UnityEngine;
using DG.Tweening;

public class TiltSignEffect : MonoBehaviour
{
    public float delay = 0.3f;
    public float angle = 15f;
    public float duration = 0.6f;

    private Quaternion baseRot;

void OnEnable()
{
    transform.localRotation = baseRot;

    DOVirtual.DelayedCall(delay, PlayTilt);
}

    void Awake()
    {
        baseRot = transform.localRotation;
    }

    // 🔥 外から呼ぶ
    public void Play()
    {
        transform.localRotation = baseRot;

        DOVirtual.DelayedCall(delay, PlayTilt);
    }

    void PlayTilt()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform
            .DOLocalRotate(new Vector3(0, 0, angle), duration)
            .SetEase(Ease.OutCubic));

        seq.Append(transform
            .DOLocalRotate(new Vector3(0, 0, angle * 0.7f), 0.2f));

        seq.Append(transform
            .DOLocalRotate(new Vector3(0, 0, angle), 0.15f));
    }
}