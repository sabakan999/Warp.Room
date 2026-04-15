using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BeamCannon : MonoBehaviour
{
    [Header("タイミング")]
    public float activateDelay = 2f;
    public float warningTime = 0.5f;
    public float beamDuration = 1f;

    [Header("見た目")]
    public GameObject cannonVisual;
    public GameObject warningLine;
    public Transform beamVisual;
    public GameObject beamHitbox;

    [Header("ビーム演出")]
    public float beamExtendTime = 0.15f;
    public float beamDisappearTime = 0.2f;

    [Header("発射演出")]
    public float cameraShakeDuration = 0.15f;
    public float cameraShakeStrength = 0.15f;
    public float flashAlpha = 1.5f; // 発光っぽく一瞬明るく

    [Header("砲台出現演出")]
    public float appearDistance = 1f;
    public float appearTime = 0.3f;

    private Vector3 cannonStartPos;
    private Vector3 cannonEndPos;
    private Vector3 beamOriginalScale;

    private SpriteRenderer beamRenderer;
    private Color beamOriginalColor;

    void Start()
    {
        SetupInitialState();
        StartCoroutine(FireRoutine());
    }

    void SetupInitialState()
    {
        // ⚠ 警告OFF
        if (warningLine != null)
            warningLine.SetActive(false);

        // 💥 ビーム設定
        if (beamVisual != null)
        {
            beamOriginalScale = beamVisual.localScale;

            beamRenderer = beamVisual.GetComponent<SpriteRenderer>();
            if (beamRenderer != null)
                beamOriginalColor = beamRenderer.color;

            beamVisual.gameObject.SetActive(false);

            // 左端固定で縮める
            beamVisual.localScale = new Vector3(
                0f,
                beamOriginalScale.y,
                beamOriginalScale.z
            );
        }

        // ☠ 判定OFF
        if (beamHitbox != null)
            beamHitbox.SetActive(false);

        // 🔫 砲台初期位置
        if (cannonVisual != null)
        {
            cannonEndPos = cannonVisual.transform.localPosition;
            cannonStartPos = cannonEndPos - transform.right * appearDistance;

            cannonVisual.transform.localPosition = cannonStartPos;
            cannonVisual.SetActive(false);
        }
    }

    IEnumerator FireRoutine()
    {
        // ⏳ 起動待ち
        yield return new WaitForSeconds(activateDelay);

        // 🔫 砲台出現
        if (cannonVisual != null)
        {
            cannonVisual.SetActive(true);

            cannonVisual.transform
                .DOLocalMove(cannonEndPos, appearTime)
                .SetEase(Ease.OutBack);
        }

        yield return new WaitForSeconds(appearTime);

        // ⚠ 警告表示
        if (warningLine != null)
            warningLine.SetActive(true);

        yield return new WaitForSeconds(warningTime);

        // ⚠ 警告消す
        if (warningLine != null)
            warningLine.SetActive(false);

        // 💥 発射時カメラ揺れ
        if (Camera.main != null)
        {
            Camera.main.transform.DOShakePosition(
                cameraShakeDuration,
                cameraShakeStrength,
                15,
                90,
                false,
                true
            );
        }

        // 💥 レーザー発射
        if (beamVisual != null)
        {
            beamVisual.gameObject.SetActive(true);

            // 色リセット
            if (beamRenderer != null)
            {
                beamRenderer.color = beamOriginalColor;

                // 一瞬明るく（発光っぽく）
                Color flashColor = beamOriginalColor;
                flashColor.a = Mathf.Clamp01(flashAlpha);

                beamRenderer.color = flashColor;

                beamRenderer
                    .DOColor(beamOriginalColor, 0.1f)
                    .SetEase(Ease.OutQuad);
            }

            // 横0から開始
            beamVisual.localScale = new Vector3(
                0f,
                beamOriginalScale.y,
                beamOriginalScale.z
            );

            // 横にみょーん
            beamVisual
                .DOScaleX(beamOriginalScale.x, beamExtendTime)
                .SetEase(Ease.OutCubic);
        }

        // ☠ 判定ON
        if (beamHitbox != null)
            beamHitbox.SetActive(true);

        // 維持
        yield return new WaitForSeconds(beamDuration);

        // ☠ 判定OFF
        if (beamHitbox != null)
            beamHitbox.SetActive(false);

        // 🔚 消える演出
        if (beamVisual != null)
        {
            Sequence disappear = DOTween.Sequence();

            // 縦に細く
            disappear.Join(
                beamVisual.DOScaleY(0f, beamDisappearTime)
            );

            // フェードアウト
            if (beamRenderer != null)
            {
                disappear.Join(
                    beamRenderer.DOFade(0f, beamDisappearTime)
                );
            }

            yield return disappear.WaitForCompletion();

            beamVisual.gameObject.SetActive(false);

            // 次回用リセット
            beamVisual.localScale = beamOriginalScale;

            if (beamRenderer != null)
                beamRenderer.color = beamOriginalColor;
        }
    }
}