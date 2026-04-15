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

    [Header("エンドレス演出")]
    public float endlessStackTime = 3f;          // ∞表示までの時間
    public float endlessResultWait = 2f;         // ∞表示後の待機
    public int endlessSpeedLockCount = 9;        // 何個積んだ時点の速度で固定するか

    [Header("音")]
    public AudioSource audioSource;
    public AudioClip baseNote;
    public AudioClip finishSound;
    public AudioClip infinitySound;

    [Header("ピッチ設定")]
    public float startPitch = 0.8f;
    public float pitchStep = 0.08f;
    public float maxPitch = 2.0f;

    [Header("エンドレス用最大ピッチ")]
    public float endlessMaxPitch = 1.6f;

    [Header("UI")]
    public Text roomText;

    [Header("カメラ")]
    public Transform camTransform;
    public float cameraSpeed = 0f;
    public float cameraSpeedStep = 0.5f;

    private bool cameraMoving = false;

    void Start()
    {
        if (roomText != null)
            roomText.gameObject.SetActive(false);

        StartCoroutine(StackRoutine());
    }

    void Update()
    {
        if (cameraMoving && camTransform != null)
        {
            camTransform.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
    }

    IEnumerator StackRoutine()
    {
        float delay = baseDelay;
        float currentPitch = startPitch;

        // =========================
        // 通常モード
        // =========================
        if (!GameSettings.isEndlessMode)
        {
            int count = GameSettings.selectedStage * 3;

            for (int i = 0; i < count; i++)
            {
                SpawnBlock(i);

                HandleCameraAndSpeed(i, ref delay, false);

                PlayStackSound(ref currentPitch, maxPitch);

                yield return new WaitForSeconds(delay);
            }

            StopCamera();
            PlayFinish();
            ShowRoomText(count.ToString());

            yield return new WaitForSeconds(1.2f);
        }
        // =========================
        // エンドレスモード
        // =========================
        else
        {
            float timer = 0f;
            int i = 0;

            // ∞表示まで積む
            while (timer < endlessStackTime)
            {
                SpawnBlock(i);

                HandleCameraAndSpeed(i, ref delay, true);

                PlayStackSound(ref currentPitch, endlessMaxPitch);

                yield return new WaitForSeconds(delay);

                timer += delay;
                i++;
            }

            // ∞表示＆専用SE
            PlayInfinity();
            ShowRoomText("∞");

            // ∞表示後もそのまま積み続ける（速度固定）
            float resultTimer = 0f;

            while (resultTimer < endlessResultWait)
            {
                SpawnBlock(i);

                // 速度固定のまま追加
                if (i == 2 && !cameraMoving)
                {
                    cameraMoving = true;
                    cameraSpeed = 1.0f;
                }

                PlayStackSound(ref currentPitch, endlessMaxPitch);

                yield return new WaitForSeconds(delay);

                resultTimer += delay;
                i++;
            }

            StopCamera();
        }

        SceneManager.LoadScene("ワープ・ルーム");
    }

    void SpawnBlock(int index)
    {
        if (blockPrefab == null || parent == null)
            return;

        float y = index * (blockHeight + spacing);

        GameObject block = Instantiate(blockPrefab, parent);
        block.transform.localPosition = new Vector3(0f, y, 0f);

        block.transform.localScale = Vector3.zero;
        block.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
    }

    void HandleCameraAndSpeed(int index, ref float delay, bool isEndless)
    {
        // 3個目からカメラ開始
        if (index == 2)
        {
            cameraMoving = true;
            cameraSpeed = 1.0f;
        }

        // 3個ごとに加速
        if ((index + 1) % 3 == 0 && cameraMoving)
        {
            // エンドレス時、9個積んだら以降固定
            if (isEndless && index + 1 > endlessSpeedLockCount)
                return;

            cameraSpeed += cameraSpeedStep;
            delay *= speedUpRate;
        }
    }

    void PlayStackSound(ref float currentPitch, float pitchLimit)
    {
        if (audioSource == null || baseNote == null)
            return;

        audioSource.pitch = currentPitch;
        audioSource.PlayOneShot(baseNote);

        currentPitch += pitchStep;
        currentPitch = Mathf.Min(currentPitch, pitchLimit);
    }

    void PlayFinish()
    {
        if (audioSource != null && finishSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(finishSound);
        }

        if (camTransform != null)
        {
            camTransform.DOShakePosition(0.3f, 0.2f);
        }
    }

    void PlayInfinity()
    {
        if (audioSource != null)
        {
            audioSource.pitch = 1f;

            if (infinitySound != null)
                audioSource.PlayOneShot(infinitySound);
            else if (finishSound != null)
                audioSource.PlayOneShot(finishSound);
        }

        if (camTransform != null)
        {
            camTransform.DOShakePosition(0.4f, 0.3f);
        }
    }

    void StopCamera()
    {
        cameraMoving = false;
        cameraSpeed = 0f;
    }

    void ShowRoomText(string text)
    {
        if (roomText == null)
            return;

        roomText.text = text;
        roomText.transform.localScale = Vector3.zero;
        roomText.gameObject.SetActive(true);

        roomText.transform
            .DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                roomText.transform.DOScale(1f, 0.1f);
            });
    }
}