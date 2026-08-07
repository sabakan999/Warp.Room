using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;

    public Image portrait;
    public Text speakerName;
    public Text message;

    private string[] messages;
    private int currentIndex;

    public bool IsTalking { get; private set; }

    void Awake()
    {
        // 起動時は自分で非表示
        if (root != null)
            root.SetActive(false);

        IsTalking = false;
    }

    public void StartDialogue(Sprite face, string name, string[] texts)
    {
        if (root != null)
            root.SetActive(true);

        portrait.sprite = face;
        speakerName.text = name;

        messages = texts;
        currentIndex = 0;

        message.text = messages[currentIndex];

        IsTalking = true;
    }

    public bool NextMessage()
    {
        currentIndex++;

        if (currentIndex >= messages.Length)
        {
            Hide();
            return true;
        }

        message.text = messages[currentIndex];
        return false;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        IsTalking = false;
    }
}