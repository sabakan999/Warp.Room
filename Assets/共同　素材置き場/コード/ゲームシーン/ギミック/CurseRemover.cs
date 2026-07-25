using UnityEngine;

public class CurseRemover : MonoBehaviour
{
    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip removeSE;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null)
            return;

        // 呪いを持っていなければ何もしない
        if (!health.hasCurse)
            return;

        // 呪い解除
        health.RemoveCurse();

        // 効果音
        if (audioSource != null && removeSE != null)
            audioSource.PlayOneShot(removeSE);

        Debug.Log("呪い解除");

        // 効果音が鳴り終わってから消す
        if (removeSE != null)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, removeSE.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}