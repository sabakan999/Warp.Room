using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class BackSceneManager : MonoBehaviour
{
    [Header("戻り先シーン")]
    public string backSceneName = "モードセレクト";

    [Header("戻るキー")]
    public KeyCode backKey = KeyCode.Backspace;

    [Header("ボタン")]
    [SerializeField] private RectTransform buttonImage;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip backSE;

    private bool isTransitioning = false;

    void Update()
    {
        if (isTransitioning)
            return;

        if (Input.GetKeyDown(backKey) ||
            Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            StartCoroutine(ReturnCoroutine());
        }
    }

    IEnumerator ReturnCoroutine()
    {
        isTransitioning = true;

        if (buttonImage != null)
        {
            Sequence seq = DOTween.Sequence()
                .SetLink(gameObject);

            seq.Append(buttonImage.DOScale(0.9f, 0.08f));
            seq.Join(buttonImage.DOAnchorPosY(
                buttonImage.anchoredPosition.y - 8,
                0.08f));

            seq.Append(buttonImage.DOScale(1f, 0.08f));
            seq.Join(buttonImage.DOAnchorPosY(
                buttonImage.anchoredPosition.y,
                0.08f));

            yield return seq.WaitForCompletion();
        }

        if (audioSource != null && backSE != null)
            audioSource.PlayOneShot(backSE);

        float wait = (backSE != null) ? backSE.length : 0.2f;

        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene(backSceneName);
    }
}