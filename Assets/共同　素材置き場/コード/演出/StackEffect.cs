using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class StackEffect : MonoBehaviour
{
    [Header("ブロック")]
    public GameObject blockPrefab;
    public Transform parent;
    public float blockHeight = 1.0f;

    [Header("ブロック間隔")]
    public float spacing = 0.2f;

    [Header("演出速度")]
    public float baseDelay = 0.25f;
    public float speedUpRate = 0.85f;

    [Header("音")]
    public AudioSource audioSource;
    public AudioClip baseNote;
    public AudioClip finishSound;

    [Header("ピッチ設定")]
    public float startPitch = 0.8f;
    public float pitchStep = 0.08f;
    public float maxPitch = 2.0f;

    [Header("UI")]
    public Text roomText;

    [Header("カメラ")]
    public Transform camTransform;
    public float cameraOffset = 2.5f;
    public float cameraSpeed = 0f;
    public float cameraSpeedStep = 0.5f;

    private bool cameraMoving = false;

    void Start()
    {
        roomText.gameObject.SetActive(false);
        StartCoroutine(StackRoutine());
    }

    void Update()
    {
        // 🎥 カメラ上昇
        if (cameraMoving)
        {
            camTransform.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
    }

    IEnumerator StackRoutine()
    {
        int count = GameSettings.selectedStage * 3;
        float delay = baseDelay;
        float currentPitch = startPitch;

        for (int i = 0; i < count; i++)
        {
            float y = i * (blockHeight + spacing);

            // 🧱 ブロック生成
            GameObject block = Instantiate(blockPrefab, parent);
            block.transform.localPosition = new Vector3(0, y, 0);

            block.transform.localScale = Vector3.zero;
            block.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);

            // 🎥 カメラ開始（3個後）
            if (i == 2)
            {
                cameraMoving = true;
                cameraSpeed = 1.0f;
            }

            // 🎥 カメラ加速（3個ごと）
            if ((i + 1) % 3 == 0 && cameraMoving)
            {
                cameraSpeed += cameraSpeedStep;
            }

            // 🎵 音
            audioSource.pitch = currentPitch;
            audioSource.PlayOneShot(baseNote);

            currentPitch += pitchStep;
            currentPitch = Mathf.Min(currentPitch, maxPitch);

            // ⚡ ブロック速度加速
            if ((i + 1) % 3 == 0)
                delay *= speedUpRate;

            yield return new WaitForSeconds(delay);
        }

        // 🎥 ★ここで即停止（重要）
        cameraMoving = false;
        cameraSpeed = 0f;

        // 🎉 最後
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(finishSound);

        roomText.text = count.ToString();
        roomText.transform.localScale = Vector3.zero;
        roomText.gameObject.SetActive(true);

        roomText.transform
            .DOScale(1.2f, 0.2f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                roomText.transform.DOScale(1f, 0.1f);
            });

        camTransform.DOShakePosition(0.3f, 0.2f);

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene("ワープ・ルーム");
    }
}