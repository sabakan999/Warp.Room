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
    public float endlessStackTime = 3f;
    public float endlessResultWait = 2f;
    public int endlessSpeedLockCount = 9;

    [Header("音（単発）")]
    public AudioSource audioSource;
    public AudioClip baseNote;
    public AudioClip finishSound;
    public AudioClip infinitySound;
    public AudioClip resultSound; // ★追加

    [Header("ループ音（ドラムロール）")]
    public AudioSource loopSource; // ★追加
    public AudioClip drumRoll;     // ★追加

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

        // 🎵 ドラムロール開始
        StartDrumRoll();

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
            StopDrumRoll(); // ★追加

            PlayFinish();
            PlayResultSound(); // ★追加

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

            while (timer < endlessStackTime)
            {
                SpawnBlock(i);

                HandleCameraAndSpeed(i, ref delay, true);

                PlayStackSound(ref currentPitch, endlessMaxPitch);

                yield return new WaitForSeconds(delay);

                timer += delay;
                i++;
            }

            // ∞表示
            StopDrumRoll(); // ★追加

            PlayInfinity();
            PlayResultSound(); // ★追加

            ShowRoomText("∞");

            // 表示後も積む
            float resultTimer = 0f;

            while (resultTimer < endlessResultWait)
            {
                SpawnBlock(i);

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

    // =========================
    // 🧱 ブロック生成
    // =========================
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

    // =========================
    // 🎥 カメラ＆加速
    // =========================
    void HandleCameraAndSpeed(int index, ref float delay, bool isEndless)
    {
        if (index == 2)
        {
            cameraMoving = true;
            cameraSpeed = 1.0f;
        }

        if ((index + 1) % 3 == 0 && cameraMoving)
        {
            if (isEndless && index + 1 > endlessSpeedLockCount)
                return;

            cameraSpeed += cameraSpeedStep;
            delay *= speedUpRate;
        }
    }

    // =========================
    // 🎵 積み音
    // =========================
    void PlayStackSound(ref float currentPitch, float pitchLimit)
    {
        if (audioSource == null || baseNote == null)
            return;

        audioSource.pitch = currentPitch;
        audioSource.PlayOneShot(baseNote);

        currentPitch += pitchStep;
        currentPitch = Mathf.Min(currentPitch, pitchLimit);
    }

    // =========================
    // 🥁 ドラムロール
    // =========================
    void StartDrumRoll()
    {
        if (loopSource != null && drumRoll != null)
        {
            loopSource.clip = drumRoll;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    void StopDrumRoll()
    {
        if (loopSource != null)
        {
            loopSource.Stop();
        }
    }

    // =========================
    // 🔔 結果音
    // =========================
    void PlayResultSound()
    {
        if (audioSource != null && resultSound != null && !GameSettings.isEndlessMode)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(resultSound);
        }
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

    // =========================
    // 🎥 カメラ停止
    // =========================
    void StopCamera()
    {
        cameraMoving = false;
        cameraSpeed = 0f;
    }

    // =========================
    // 📝 テキスト表示
    // =========================
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