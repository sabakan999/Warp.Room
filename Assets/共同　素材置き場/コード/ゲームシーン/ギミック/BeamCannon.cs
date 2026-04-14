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
    public GameObject beamVisual;
    public GameObject beamHitbox;

    [Header("砲台出現演出")]
    public float appearDistance = 1f;
    public float appearTime = 0.3f;

    private Vector3 cannonStartPos;
    private Vector3 cannonEndPos;

    void Start()
    {
        SetupInitialState();
        StartCoroutine(FireRoutine());
    }

    void SetupInitialState()
    {
        // 警告OFF
        if (warningLine != null)
            warningLine.SetActive(false);

        // ビームOFF
        if (beamVisual != null)
            beamVisual.SetActive(false);

        // 当たり判定OFF
        if (beamHitbox != null)
            beamHitbox.SetActive(false);

        // 砲台初期位置
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

        // 💥 ビームON
        if (beamVisual != null)
            beamVisual.SetActive(true);

        if (beamHitbox != null)
            beamHitbox.SetActive(true);

        yield return new WaitForSeconds(beamDuration);

        // 🔚 消える
        if (beamVisual != null)
            beamVisual.SetActive(false);

        if (beamHitbox != null)
            beamHitbox.SetActive(false);
    }
}