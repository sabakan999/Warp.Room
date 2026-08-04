using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class NameInputManager : MonoBehaviour
{
    [Header("表示")]
    public Text nameText;
    public Text parentText;
    public Text childText;

    [Header("カーソル(Image)")]
    public RectTransform parentCursor;
    public RectTransform childCursor;

    [Header("カーソル位置")]
    public float parentSpacing = 90f;
    public float childSpacing = 60f;

    [Header("シーン")]
    public string nextScene = "演出";

    [Header("子音パネル")]
    public RectTransform childPanel;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip cursorSE;
    public AudioClip selectSE;
    public AudioClip decideSE;

    private readonly string[] parentList =
    {
        "あ","か","さ","た","な",
        "は","ま","や","ら","わ",
        "記","消","決"
    };

    private Dictionary<int, string[]> childTable;

    private int parentIndex = 0;
    private int childIndex = 0;

    private string playerName = "";

    const int MaxLength = 6;

    // カーソル初期位置
    private Vector2 parentCursorStartPos;
    private Vector2 childCursorStartPos;

    private Vector2 childPanelStartPos;

    void Start()
    {
        CreateTable();

        if (parentCursor != null)
            parentCursorStartPos = parentCursor.anchoredPosition;

        if (childCursor != null)
            childCursorStartPos = childCursor.anchoredPosition;

        if (childPanel != null)
            childPanelStartPos = childPanel.anchoredPosition;

        RefreshUI();
    }

    void Update()
    {
        HandleHorizontal();
        HandleVertical();
        HandleSubmit();
        
    }


    void CreateTable()
    {
        childTable = new Dictionary<int, string[]>();

        childTable[0] = new string[]
        {
            "あ","い","う","え","お"
        };

        childTable[1] = new string[]
        {
            "か","き","く","け","こ"
        };

        childTable[2] = new string[]
        {
            "さ","し","す","せ","そ"
        };

        childTable[3] = new string[]
        {
            "た","ち","つ","て","と"
        };

        childTable[4] = new string[]
        {
            "な","に","ぬ","ね","の"
        };

        childTable[5] = new string[]
        {
            "は","ひ","ふ","へ","ほ"
        };

        childTable[6] = new string[]
        {
            "ま","み","む","め","も"
        };

        childTable[7] = new string[]
        {
            "や","ゆ","よ"
        };

        childTable[8] = new string[]
        {
            "ら","り","る","れ","ろ"
        };

        childTable[9] = new string[]
        {
            "わ","を","ん"
        };

        childTable[10] = new string[]
        {
            "゛","゜","小","ー"
        };

        childTable[11] = new string[]
        {
            "削除"
        };

        childTable[12] = new string[]
        {
            "決定"
        };
        }

    void HandleHorizontal()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            parentIndex--;

            if (parentIndex < 0)
                parentIndex = parentList.Length - 1;

            childIndex = 0;
            if (audioSource != null && cursorSE != null)
                audioSource.PlayOneShot(cursorSE);
            RefreshUI();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            parentIndex++;

            if (parentIndex >= parentList.Length)
                parentIndex = 0;

            childIndex = 0;
            if (audioSource != null && cursorSE != null)
                audioSource.PlayOneShot(cursorSE);
            RefreshUI();
        }
    }

    void HandleVertical()
    {
        string[] list = childTable[parentIndex];

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            childIndex--;

            if (childIndex < 0)
                childIndex = list.Length - 1;
            if (audioSource != null && cursorSE != null)
                audioSource.PlayOneShot(cursorSE);
            RefreshUI();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            childIndex++;

            if (childIndex >= list.Length)
                childIndex = 0;

            if (audioSource != null && cursorSE != null)
                audioSource.PlayOneShot(cursorSE);
            RefreshUI();
        }
    }
    void HandleSubmit()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (audioSource != null && selectSE != null)
            audioSource.PlayOneShot(selectSE);

        string selected = childTable[parentIndex][childIndex];

        switch (selected)
        {
            case "゛":
                AddDakuten();
                break;

            case "゜":
                AddHandakuten();
                break;

            case "削除":

                if (playerName.Length > 0)
                {
                    playerName = playerName.Substring(0, playerName.Length - 1);
                }

                break;

            case "決定":

                 if (playerName.Length > 0)
                {   
                    if (audioSource != null && decideSE != null)
                        audioSource.PlayOneShot(decideSE);

                    StartCoroutine(DecideRoutine());
                }
                break;
            case "小":
                AddSmallKana();
            

                break;

            default:

                if (playerName.Length < MaxLength)
                {
                    playerName += selected;
                }

                break;
        }

        RefreshUI();
    }



    void AddDakuten()
    {
        if (playerName.Length == 0)
            return;

        Dictionary<char, char> table = new Dictionary<char, char>()
        {
            {'か','が'},
            {'き','ぎ'},
            {'く','ぐ'},
            {'け','げ'},
            {'こ','ご'},

            {'さ','ざ'},
            {'し','じ'},
            {'す','ず'},
            {'せ','ぜ'},
            {'そ','ぞ'},

            {'た','だ'},
            {'ち','ぢ'},
            {'つ','づ'},
            {'て','で'},
            {'と','ど'},

            {'は','ば'},
            {'ひ','び'},
            {'ふ','ぶ'},
            {'へ','べ'},
            {'ほ','ぼ'}
        };

        char last = playerName[playerName.Length - 1];

        if (table.ContainsKey(last))
        {
            playerName = playerName.Remove(playerName.Length - 1);
            playerName += table[last];
        }
    }

    void AddHandakuten()
    {
        if (playerName.Length == 0)
            return;

        Dictionary<char, char> table = new Dictionary<char, char>()
        {
            {'は','ぱ'},
            {'ひ','ぴ'},
            {'ふ','ぷ'},
            {'へ','ぺ'},
            {'ほ','ぽ'}
        };

        char last = playerName[playerName.Length - 1];

        if (table.ContainsKey(last))
        {
            playerName = playerName.Remove(playerName.Length - 1);
            playerName += table[last];
        }
    }

    void AddSmallKana()
    {
        if (playerName.Length == 0)
            return;

        Dictionary<char, char> table = new Dictionary<char, char>()
        {
            {'あ','ぁ'},
            {'い','ぃ'},
            {'う','ぅ'},
            {'え','ぇ'},
            {'お','ぉ'},

            {'や','ゃ'},
            {'ゆ','ゅ'},
            {'よ','ょ'},

            {'つ','っ'}
        };

        char last = playerName[playerName.Length - 1];

        if (table.ContainsKey(last))
        {
            playerName = playerName.Remove(playerName.Length - 1);
            playerName += table[last];
        }
    }
    void RefreshUI()
    {
        // =========================
        // 名前表示
        // =========================
        string display = playerName;

        while (display.Length < MaxLength)
            display += "□";

        if (nameText != null)
            nameText.text = display;

        // =========================
        // 親文字表示
        // =========================
        if (parentText != null)
        {
            parentText.text = "";

            for (int i = 0; i < parentList.Length; i++)
            {
                parentText.text += parentList[i];

                if (i != parentList.Length - 1)
                    parentText.text += "　";
            }
        }

    

        // =========================
        // 子文字表示
        // =========================
        if (childText != null)
        {
            childText.text = "";

            string[] list = childTable[parentIndex];

            for (int i = 0; i < list.Length; i++)
            {
                childText.text += list[i];

                if (i != list.Length - 1)
                    childText.text += "\n";
            }
        }

        // =========================
        // 親カーソル移動
        // =========================
        if (parentCursor != null)
        {
            Vector2 pos = parentCursorStartPos;

            pos.x += parentIndex * parentSpacing;

            parentCursor.anchoredPosition = pos;
        }
        if (childPanel != null)
    {
        Vector2 panelPos = childPanelStartPos;

        panelPos.x =
            parentCursor.anchoredPosition.x;

        childPanel.anchoredPosition = panelPos;
    }

        // =========================
        // 子カーソル移動
        // =========================
        if (childCursor != null)
        {
            Vector2 pos = childCursorStartPos;

            pos.y -= childIndex * childSpacing;

            childCursor.anchoredPosition = pos;
        }
    }
    IEnumerator DecideRoutine()
    {
        float wait = (decideSE != null) ? decideSE.length : 0.2f;

        yield return new WaitForSeconds(wait);
        GameSettings.playerName = playerName;

        SceneManager.LoadScene(nextScene);
    }
}