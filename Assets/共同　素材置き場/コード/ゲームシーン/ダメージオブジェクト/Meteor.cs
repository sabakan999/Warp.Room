using UnityEngine;
using DG.Tweening;

public class Meteor : MonoBehaviour
{
    [Header("移動")]
    public float speed = 18f;

    [Header("生成演出")]
    public float growTime = 0.35f;

    [Header("回転")]
    public float rotateSpeed = 720f;

    [Header("寿命")]
    public float lifeTime = 5f;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip launchSE;

    [HideInInspector]
    public Transform target;

    Vector3 direction;

    bool launched = false;
    private Vector3 originalScale;

    void Start()
{
    // プレハブ本来のサイズを保存
    originalScale = transform.localScale;

    // 0から開始
    transform.localScale = Vector3.zero;

    transform
        .DOScale(originalScale, growTime)
        .SetEase(Ease.OutBack)
        .OnComplete(Launch)
        .SetLink(gameObject);

    Destroy(gameObject, lifeTime);
}
    void Launch()
    {

        MeteorSpawner spawner = GetComponentInParent<MeteorSpawner>();

        GameManager gm = FindFirstObjectByType<GameManager>();

        if (spawner != null &&
            spawner.audioSource != null)
        {
            spawner.audioSource.Stop();
        }

        // 発射SE
        if (audioSource != null && launchSE != null && gm != null && gm.isGameRunning)
        {
            audioSource.PlayOneShot(launchSE);
        }
        if (target != null)
        {
            direction =
                (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector3.down;
        }

        launched = true;
    }

    void Update()
    {
        if (!launched)
            return;

        transform.position +=
            direction * speed * Time.deltaTime;

        transform.Rotate(
            0,
            0,
            rotateSpeed * Time.deltaTime);
    }
}