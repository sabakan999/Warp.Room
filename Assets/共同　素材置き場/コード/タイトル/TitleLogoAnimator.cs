using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TitleLogoAnimator : MonoBehaviour
{
    private Vector3 basePos;
    private Vector3 baseScale;
    private Quaternion baseRot;

    float BIG = 1200f; // 画面外ぶっ飛び用

    void OnEnable()
    {
        basePos = transform.localPosition;
        baseScale = transform.localScale;
        baseRot = transform.localRotation;

        StartCoroutine(RandomActionLoop());
    }

    IEnumerator RandomActionLoop()
    {
        while (true)
        {
            float wait = Random.Range(2f, 5f);
            yield return new WaitForSeconds(wait);

            yield return StartCoroutine(PlayRandomAction());

            ResetTransform();
        }
    }

    IEnumerator PlayRandomAction()
    {
        int rand = Random.Range(0, 19);

        switch (rand)
        {
            // 🔹短い
            case 0: yield return PunchScale(); break;
            case 1: yield return JumpBounce(); break;
            case 2: yield return Spin(); break;
            case 3: yield return SquashStretch(); break;
            case 4: yield return Wiggle(); break;
            case 5: yield return SlideAndBack(); break;
            case 6: yield return Flip(); break;
            case 7: yield return PopRotate(); break;

            // 🔥長編
            case 8: yield return Story_JumpAndSmash(); break;
            case 9: yield return Story_LostAndReturn(); break;
            case 10: yield return Story_PanicShake(); break;
            case 11: yield return Story_BigBounce(); break;
            case 12: yield return Story_SpinFlyAway(); break;
            case 13: yield return Story_GrowAndBurst(); break;

            // 🧠 人間っぽい
            case 14: yield return Story_DoZeAndWake(); break;
            case 15: yield return Story_Sneeze(); break;
            case 16: yield return Story_Stretch(); break;
            case 17: yield return Story_RunAway(); break;
            case 18: yield return Story_FlyUp(); break;
        }
    }

    // =========================
    // 🎪 短い演出
    // =========================

    IEnumerator PunchScale()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(baseScale * 0.7f, 0.2f));
        seq.Append(transform.DOScale(baseScale * 1.4f, 0.3f).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(baseScale, 0.2f));
        yield return seq.WaitForCompletion();
    }

    IEnumerator JumpBounce()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(basePos.y + 40f, 0.3f));
        seq.Append(transform.DOLocalMoveY(basePos.y, 0.3f));
        seq.Append(transform.DOLocalMoveY(basePos.y + 20f, 0.2f));
        seq.Append(transform.DOLocalMoveY(basePos.y, 0.2f));
        yield return seq.WaitForCompletion();
    }

    IEnumerator Spin()
    {
        yield return transform
            .DOLocalRotate(new Vector3(0, 0, 360f), 0.8f, RotateMode.FastBeyond360)
            .WaitForCompletion();
    }

    IEnumerator SquashStretch()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(new Vector3(1.3f, 0.7f, 1f), 0.2f));
        seq.Append(transform.DOScale(new Vector3(0.7f, 1.3f, 1f), 0.2f));
        seq.Append(transform.DOScale(baseScale, 0.2f));
        yield return seq.WaitForCompletion();
    }

    IEnumerator Wiggle()
    {
        yield return transform
            .DOShakeRotation(0.5f, new Vector3(0, 0, 20f))
            .WaitForCompletion();
    }

    IEnumerator SlideAndBack()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(basePos + new Vector3(150f, 0f, 0f), 0.2f));
        seq.Append(transform.DOLocalMove(basePos, 0.3f));
        yield return seq.WaitForCompletion();
    }

    IEnumerator Flip()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleX(0f, 0.2f));
        seq.Append(transform.DOScaleX(baseScale.x, 0.2f));
        yield return seq.WaitForCompletion();
    }

    IEnumerator PopRotate()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(baseScale * 1.3f, 0.2f));
        seq.Join(transform.DOLocalRotate(new Vector3(0, 0, 90f), 0.2f));
        seq.Append(transform.DOScale(baseScale, 0.2f));
        seq.Join(transform.DOLocalRotate(Vector3.zero, 0.2f));
        yield return seq.WaitForCompletion();
    }

    // =========================
    // 🎬 長編
    // =========================

    IEnumerator Story_JumpAndSmash()
    {
        yield return transform.DOLocalMoveY(basePos.y + BIG, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(1.5f);
        yield return transform.DOLocalMoveY(basePos.y, 0.3f).SetEase(Ease.InQuad).WaitForCompletion();
        yield return transform.DOScale(new Vector3(1.5f, 0.5f, 1f), 0.2f).WaitForCompletion();
        yield return transform.DOScale(baseScale, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
    }

   IEnumerator Story_LostAndReturn()
{
    float offscreenX = 2000f; // ← 好きなだけ増やしてOK（1500〜3000推奨）

    // 画面外へぶっ飛び
    yield return transform
        .DOLocalMoveX(offscreenX, 0.5f)
        .SetEase(Ease.InQuad)
        .WaitForCompletion();

    yield return new WaitForSeconds(1f);

    // 左から出現
    transform.localPosition = new Vector3(-offscreenX, basePos.y, 0);

    yield return transform
        .DOLocalMove(basePos, 1f)
        .SetEase(Ease.OutCubic)
        .WaitForCompletion();
}

    IEnumerator Story_PanicShake()
    {
        yield return transform.DOShakePosition(2f, 20f).WaitForCompletion();
    }

    IEnumerator Story_BigBounce()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return transform.DOLocalMoveY(basePos.y + 200f, 0.3f).WaitForCompletion();
            yield return transform.DOLocalMoveY(basePos.y, 0.3f).WaitForCompletion();
        }
    }

    IEnumerator Story_SpinFlyAway()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(transform.DOLocalMove(basePos + new Vector3(BIG, BIG * 0.6f, 0), 1f));
        seq.Join(transform.DOLocalRotate(new Vector3(0, 0, 720), 1f));

        yield return seq.WaitForCompletion();

        yield return new WaitForSeconds(1f);

        yield return transform.DOLocalMove(basePos, 1f).WaitForCompletion();
    }

    IEnumerator Story_GrowAndBurst()
    {
        yield return transform.DOScale(baseScale * 2f, 0.5f).WaitForCompletion();
        yield return transform.DOScale(baseScale * 0.3f, 0.2f).WaitForCompletion();
        yield return transform.DOScale(baseScale, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
    }

    // =========================
    // 🧠 人間っぽい
    // =========================

    IEnumerator Story_DoZeAndWake()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveY(basePos.y - 30f, 1.2f));
        seq.Join(transform.DOLocalRotate(new Vector3(0, 0, 10f), 1.2f));

        seq.Append(transform.DOLocalMoveY(basePos.y - 80f, 0.15f));
        seq.Join(transform.DOLocalRotate(new Vector3(0, 0, -15f), 0.15f));

        seq.Append(transform.DOLocalMoveY(basePos.y + 40f, 0.2f));
        seq.Join(transform.DOLocalRotate(Vector3.zero, 0.2f));

        seq.Append(transform.DOLocalMove(basePos, 0.4f));

        yield return seq.WaitForCompletion();
    }

    IEnumerator Story_Sneeze()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(baseScale * 0.8f, 0.4f));
        seq.Append(transform.DOLocalMoveX(basePos.x + 200f, 0.1f));
        seq.Join(transform.DOScale(baseScale * 1.5f, 0.1f));
        seq.Append(transform.DOLocalMoveX(basePos.x - 100f, 0.2f));
        seq.Append(transform.DOLocalMove(basePos, 0.4f));
        seq.Join(transform.DOScale(baseScale, 0.4f));

        yield return seq.WaitForCompletion();
    }

    IEnumerator Story_Stretch()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(new Vector3(1f, 1.6f, 1f), 0.6f));
        seq.Append(transform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.2f));
        seq.Append(transform.DOScale(baseScale, 0.4f).SetEase(Ease.OutBack));

        yield return seq.WaitForCompletion();
    }

    IEnumerator Story_RunAway()
    {
        yield return transform.DOLocalMoveX(basePos.x + BIG, 0.6f).SetEase(Ease.InQuad).WaitForCompletion();

        yield return new WaitForSeconds(0.5f);

        transform.localPosition = new Vector3(basePos.x - BIG, basePos.y, 0);

        yield return transform.DOLocalMove(basePos, 0.4f).SetEase(Ease.OutExpo).WaitForCompletion();
    }

    IEnumerator Story_FlyUp()
    {
        yield return transform.DOLocalMoveY(basePos.y + BIG, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();

        yield return new WaitForSeconds(1f);

        transform.localPosition = new Vector3(basePos.x, basePos.y + BIG, 0);

        yield return transform.DOLocalMove(basePos, 0.6f).SetEase(Ease.InQuad).WaitForCompletion();
    }

    // =========================
    // 🔄 リセット
    // =========================
    void ResetTransform()
    {
        transform.DOKill();
        transform.localPosition = basePos;
        transform.localScale = baseScale;
        transform.localRotation = baseRot;
    }
}