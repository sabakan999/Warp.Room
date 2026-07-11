using UnityEngine;
using DG.Tweening;

public class AreaEffect : MonoBehaviour
{
    public enum EffectType
    {
        DisableJump,
        DisableMove
    }

    [Header("効果")]
    public EffectType effect;

    [Header("出現演出")]
    public float startScale = 2.5f;
    public float appearTime = 0.2f;

    [Header("SE")]
    public AudioClip appearSE;
    public AudioSource audioSource;

    private Vector3 originalScale;

    void Start()
    {
        // 元のサイズを保存
        originalScale = transform.localScale;

        // 最初は大きく
        transform.localScale = originalScale * startScale;

        // ズームアウト演出
        transform.DOScale(originalScale, appearTime)
            .SetEase(Ease.OutCubic);

      // 効果音
if (audioSource != null && appearSE != null)
{
    audioSource.PlayOneShot(appearSE);
}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        ApplyEffect(player, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        ApplyEffect(player, false);
    }

    void ApplyEffect(PlayerController player, bool enable)
    {
        switch (effect)
        {
            case EffectType.DisableJump:
                player.SetJumpEnabled(!enable);
                break;

            case EffectType.DisableMove:
                player.SetMoveEnabled(!enable);
                break;

            
        }
    }
}