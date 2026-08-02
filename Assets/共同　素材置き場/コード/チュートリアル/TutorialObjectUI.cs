using UnityEngine;

public class TutorialObjectUI : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void Show()
    {
        if (target != null)
            target.SetActive(true);
    }

    public void Hide()
    {
        if (target != null)
            target.SetActive(false);
    }

    public void Toggle()
    {
        if (target != null)
            target.SetActive(!target.activeSelf);
    }
}