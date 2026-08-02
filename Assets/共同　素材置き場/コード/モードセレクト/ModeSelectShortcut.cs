using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class ModeSelectShortcut : MonoBehaviour
{
    [Header("遷移先シーン")]
    public string tutorialSceneName = "チュートリアル確認";

    [Header("チュートリアルキー")]
    public KeyCode tutorialKey = KeyCode.Escape;

    [Header("ボタン")]
    [SerializeField] private RectTransform buttonImage;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip selectSE;

    private bool isTransitioning = false;

    void Update()
    {
        if (isTransitioning)
            return;

        if (Input.GetKeyDown(tutorialKey))
        {
            StartCoroutine(TutorialCoroutine());
        }
    }

    IEnumerator TutorialCoroutine()
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

        if (audioSource != null && selectSE != null)
            audioSource.PlayOneShot(selectSE);

        float wait = (selectSE != null) ? selectSE.length : 0.2f;

        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene(tutorialSceneName);
    }
}