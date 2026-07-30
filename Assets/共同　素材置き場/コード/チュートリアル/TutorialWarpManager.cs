using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWarpManager : MonoBehaviour
{
    [Header("タイマー")]
    public Image gauge;
    public float warpTime = 3f;

    private float currentTime;
    private bool isRunning;

    [Header("暗幕")]
    public GameObject blackScreen;

    [Header("生成")]
    public GameObject roomPrefab;
    public Transform roomSpawn;

    public GameObject playerPrefab;
    public Transform playerSpawn;

    [Header("現在のオブジェクト")]
    public GameObject currentRoom;
    public GameObject currentPlayer;

   [Header("SE")]
public AudioSource seSource;
public AudioClip warpStartSE;
public AudioClip warpEndSE;
    [Header("管理")]
    public TutorialManager tutorialManager;

    void Start()
    {
        gauge.transform.parent.gameObject.SetActive(false);

        if (blackScreen != null)
            blackScreen.SetActive(false);
    }

    void Update()
    {
        if (!isRunning)
            return;

        currentTime -= Time.deltaTime;

        currentTime = Mathf.Clamp(currentTime, 0, warpTime);

        gauge.fillAmount = currentTime / warpTime;

        if (currentTime <= 0)
        {
            isRunning = false;

            StartCoroutine(WarpRoutine());
        }
    }

    //==========================
    // タイマー開始
    //==========================
    public void StartWarpTimer()
    {
        currentTime = warpTime;

        gauge.fillAmount = 1f;

        gauge.transform.parent.gameObject.SetActive(true);

        isRunning = true;
    }

    //==========================
    // ワープ
    //==========================
    IEnumerator WarpRoutine()
    {
        gauge.transform.parent.gameObject.SetActive(false);

       if (warpStartSE != null)
{
    seSource.PlayOneShot(warpStartSE);
}

        if (blackScreen != null)
            blackScreen.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        if (currentPlayer != null)
            Destroy(currentPlayer);

        if (currentRoom != null)
            Destroy(currentRoom);

        yield return null;

        currentRoom = Instantiate(roomPrefab, roomSpawn.position, Quaternion.identity);

        currentPlayer = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        PlayerController player = currentPlayer.GetComponent<PlayerController>();
        tutorialManager.SetPlayer(player);

        yield return new WaitForSeconds(0.2f);
if (warpEndSE != null)
{
    seSource.PlayOneShot(warpEndSE);
}

        if (blackScreen != null)
            blackScreen.SetActive(false);

        if (tutorialManager != null)
        {
            tutorialManager.ReportWarpFinished();
        }
    }
}