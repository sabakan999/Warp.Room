using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackSceneManager : MonoBehaviour
{
    [Header("戻り先シーン")]
    public string backSceneName = "モードセレクト";

    [Header("戻るキー")]
    public KeyCode backKey = KeyCode.Backspace;

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

        if (audioSource != null && backSE != null)
            audioSource.PlayOneShot(backSE);

        float wait = (backSE != null) ? backSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene(backSceneName);
    }
}