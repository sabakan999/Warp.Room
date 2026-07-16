using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class UIOverlapFader : MonoBehaviour
{
    [Header("対象UI")]
    public RectTransform[] targetUI;

    [Header("判定するタグ")]
    public string[] targetTags;

    [Header("透明度")]
    public float normalAlpha = 1f;
    public float overlapAlpha = 0.35f;

    [Header("フェード時間")]
    public float fadeTime = 0.2f;

    private CanvasGroup[] canvasGroups;

    void Start()
    {
        canvasGroups = new CanvasGroup[targetUI.Length];

        for (int i = 0; i < targetUI.Length; i++)
        {
            canvasGroups[i] = targetUI[i].GetComponent<CanvasGroup>();

            if (canvasGroups[i] == null)
                canvasGroups[i] = targetUI[i].gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        // 判定対象を全タグから取得
        List<GameObject> targets = new List<GameObject>();

        foreach (string tag in targetTags)
        {
            if (string.IsNullOrEmpty(tag))
                continue;

            targets.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        // UIごとに判定
        for (int i = 0; i < targetUI.Length; i++)
        {
            bool overlap = false;

            foreach (GameObject obj in targets)
            {
                if (obj == null)
                    continue;

                Vector3 screenPos =
                    Camera.main.WorldToScreenPoint(obj.transform.position);

                if (RectTransformUtility.RectangleContainsScreenPoint(
                    targetUI[i],
                    screenPos,
                    null))
                {
                    overlap = true;
                    break;
                }
            }

            float targetAlpha = overlap ? overlapAlpha : normalAlpha;

            if (Mathf.Abs(canvasGroups[i].alpha - targetAlpha) > 0.01f)
            {
                canvasGroups[i]
                    .DOFade(targetAlpha, fadeTime);
            }
        }
    }
}