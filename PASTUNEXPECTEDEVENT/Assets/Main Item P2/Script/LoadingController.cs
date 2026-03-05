using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    [Header("UI Components")]
    public Image loadingImage; // 必须连接：把Hierarchy里的LoadingPicture拖到这里

    [Header("Random Images")]
    public Sprite[] imageArray; // 把Project里的图片拖到这里

    [Header("Settings")]
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public float minShowTime = 2f;
    public float maxShowTime = 4f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // 设置初始状态
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false); // 开始隐藏
    }

    public void ShowLoading()
    {
        // 1. 安全检查
        if (loadingImage == null)
        {
            Debug.LogError("【错误】loadingImage没有连接！请检查Inspector设置。");
            // 即使出错也显示画面，防止游戏卡住
            SimpleShow();
            return;
        }

        // 2. 准备显示
        CancelInvoke("HideWithFade");
        gameObject.SetActive(true);

        // 3. 随机换图
        if (imageArray != null && imageArray.Length > 0)
        {
            int randomIndex = Random.Range(0, imageArray.Length);
            loadingImage.sprite = imageArray[randomIndex];
            Debug.Log("显示图片：" + imageArray[randomIndex].name);
        }

        // 4. 淡入效果
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeInTime).SetEase(Ease.OutQuad);

        // 5. 定时淡出
        float showTime = Random.Range(minShowTime, maxShowTime);
        Invoke("HideWithFade", fadeInTime + showTime);
    }

    private void SimpleShow()
    {
        // 简化版显示（当loadingImage没连接时用）
        gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeInTime);
        Invoke("HideWithFade", fadeInTime + 3f);
    }

    private void HideWithFade()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, fadeOutTime).SetEase(Ease.InQuad)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
    }

    public void HideNow()
    {
        CancelInvoke("HideWithFade");
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}