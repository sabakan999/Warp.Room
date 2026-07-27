using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    public TutorialManager tutorialManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);

        if (!other.CompareTag("Player"))
            return;

        tutorialManager.ReportMove();

        gameObject.SetActive(false);
    }
}