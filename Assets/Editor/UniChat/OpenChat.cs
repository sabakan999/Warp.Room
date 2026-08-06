using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Unity内でChatGPT相談用プロンプトを作るEditor拡張です。
/// APIは使わず、プロンプトをコピーしてChromeでChatGPTを開く方式です。
/// </summary>
public class OpenChat : EditorWindow
{
    // ユーザーが入力する相談内容
    private string userInput = "";

    // 自動生成されたプロンプト
    private string generatedPrompt = "";

    // スクロール位置
    private Vector2 scrollPosition;

    // Chrome の一般的なインストール場所
    private const string BrowserPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

    // 開きたいURL
    private const string ChatGptUrl = "https://chatgpt.com/";

    private const string GeminiUrl = "https://gemini.google.com/";

    /// <summary>
    /// Unity上部メニューに Tools > UniChat > Prompt Assistant を追加します。
    /// </summary>
    [MenuItem("Tools/UniChat/Prompt Assistant")]
    public static void OpenWindow()
    {
        GetWindow<OpenChat>("Prompt Assistant");
    }

    /// <summary>
    /// EditorWindowの見た目を作ります。
    /// </summary>
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Prompt Assistant", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "これはプロンプトのひな形の作成と,ChatGPTかGeminiに行くだけのもの",
            MessageType.Info
        );

        GUILayout.Space(10);

        DrawInputArea();

        GUILayout.Space(10);

        DrawButtons();

        GUILayout.Space(10);

        DrawGeneratedPromptArea();

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 相談内容を入力するエリアです。
    /// </summary>
    private void DrawInputArea()
    {
        GUILayout.Label("相談内容", EditorStyles.boldLabel);

        userInput = EditorGUILayout.TextArea(
            userInput,
            GUILayout.Height(150)
        );
    }

    /// <summary>
    /// 操作用ボタンを表示します。
    /// </summary>
    private void DrawButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("プロンプト生成", GUILayout.Height(35)))
        {
            GeneratePrompt();
        }

        if (GUILayout.Button("コピー", GUILayout.Height(35)))
        {
            CopyPrompt();
        }

        if (GUILayout.Button("ChatGPTを開く", GUILayout.Height(35)))
        {
            OpenChatGPT();
        }

        if (GUILayout.Button("Geminiを開く", GUILayout.Height(35)))
        {
            OpenGemini();
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 生成されたプロンプトを表示します。
    /// </summary>
    private void DrawGeneratedPromptArea()
    {
        GUILayout.Label("生成されたプロンプト", EditorStyles.boldLabel);

        generatedPrompt = EditorGUILayout.TextArea(
            generatedPrompt,
            GUILayout.Height(250)
        );
    }

    /// <summary>
    /// 入力された相談内容からChatGPT用プロンプトを生成します。
    /// </summary>
    private void GeneratePrompt()
    {
        generatedPrompt =
$@"あなたはUnity開発のアシスタントです。
以下の相談内容について、初心者にもわかるように説明してください。

回答では次の内容を含めてください。
・原因や考えられる問題
・確認すべき場所
・修正方法
・必要であればC#コード例
・Unity Editor上での設定手順

【相談内容】
{userInput}";

        ShowNotification(new GUIContent("プロンプトを生成しました"));
    }

    /// <summary>
    /// 生成されたプロンプトをクリップボードにコピーします。
    /// </summary>
    private void CopyPrompt()
    {
        if (string.IsNullOrEmpty(generatedPrompt))
        {
            GeneratePrompt();
        }

        EditorGUIUtility.systemCopyBuffer = generatedPrompt;

        ShowNotification(new GUIContent("コピーしました"));
    }

    /// <summary>
    /// ChromeでChatGPTを開きます。
    /// Chromeが見つからない場合は、既定のブラウザで開きます。
    /// </summary>
    private void OpenChatGPT()
    {
        if (File.Exists(BrowserPath))
        {
            Process.Start(BrowserPath, ChatGptUrl);
        }
        else
        {
            Application.OpenURL(ChatGptUrl);
        }
    }

    private void OpenGemini()
    {
        if (File.Exists(BrowserPath))
        {
            Process.Start(BrowserPath, GeminiUrl);
        }
        else
        {
            Application.OpenURL(GeminiUrl);
        }
    }
}