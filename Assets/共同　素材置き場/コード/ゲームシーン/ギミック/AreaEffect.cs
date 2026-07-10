using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    public enum EffectType
    {
        DisableJump,
        DisableMove,
        DisableAttack,
        DisableDash
    }

    [Header("効果")]
    public EffectType effect;

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