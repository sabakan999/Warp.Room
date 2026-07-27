using UnityEngine;

public class TutorialJumpTarget : MonoBehaviour
{
    public TutorialManager tutorialManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        tutorialManager.ReportJump();

        gameObject.SetActive(false);
    }
}