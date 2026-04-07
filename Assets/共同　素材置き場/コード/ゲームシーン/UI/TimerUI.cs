using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image gauge;

    private float maxTime;
    private float currentTime;
    private bool isRunning = false;

    public void StartTimer(float duration)
    {
        maxTime = duration;
        currentTime = duration;
        isRunning = true;

        gameObject.SetActive(true);
    }

    public void StopTimer()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0, maxTime);

        gauge.fillAmount = currentTime / maxTime;
    }
}