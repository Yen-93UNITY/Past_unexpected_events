using System.Collections;
using UnityEngine;

public class AreaFade : MonoBehaviour
{
    [Header("拖你想显示/隐藏的SpriteRenderer")]
    public SpriteRenderer targetRenderer;

    [Header("渐变时间 (秒)")]
    public float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        // 玩家不在区域时默认显示黑色方块
        if(targetRenderer != null)
        {
            Color c = targetRenderer.color;
            c.a = 1f; // 黑色方块完全可见
            targetRenderer.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家进入，黑色方块消失（alpha 0）
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTo(0f));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家离开，黑色方块出现（alpha 1）
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTo(1f));
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = targetRenderer.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            Color c = targetRenderer.color;
            c.a = alpha;
            targetRenderer.color = c;
            yield return null;
        }

        // 确保最终 alpha 完全到位
        Color finalColor = targetRenderer.color;
        finalColor.a = targetAlpha;
        targetRenderer.color = finalColor;
    }
}

