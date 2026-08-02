using System.Collections;
using UnityEngine;

public class TutorialJumpTarget : MonoBehaviour
{
    public TutorialManager tutorialManager;

    [Header("取得SE")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip getSE;

    [SerializeField] private GameObject pickupEffectPrefab;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        isCollected = true;

        // 二重取得防止
        GetComponent<Collider2D>().enabled = false;

        // 見た目だけ消す
        GetComponent<SpriteRenderer>().enabled = false;

        //演出

        if (pickupEffectPrefab != null)
        {
            Instantiate(
                pickupEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        StartCoroutine(GetRoutine());
    }

    IEnumerator GetRoutine()
    {
        // SE再生
        if (audioSource != null && getSE != null)
        {
            audioSource.volume = OptionSettings.SEVolume;
            audioSource.PlayOneShot(getSE);

            yield return new WaitForSeconds(getSE.length);
        }

        tutorialManager.ReportJump();

        gameObject.SetActive(false);
    }
}