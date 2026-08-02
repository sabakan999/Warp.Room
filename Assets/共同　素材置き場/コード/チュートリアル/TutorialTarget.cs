using System.Collections;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
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

        StartCoroutine(GetRoutine());
    }

    IEnumerator GetRoutine()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        if (pickupEffectPrefab != null)
        {
            Instantiate(
                pickupEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }
        // SE再生
        if (audioSource != null && getSE != null)
        {
            audioSource.volume = OptionSettings.SEVolume;
            audioSource.PlayOneShot(getSE);

            // SEが終わるまで待つ
            yield return new WaitForSeconds(getSE.length);
        }

        tutorialManager.ReportMove();

        gameObject.SetActive(false);
    }
}