using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class StarterMenuSimple : MonoBehaviour
{
    [Header("UI Elements")]
    public Image blackScreenImage;      
    public TMP_Text clickPromptText;   
    public TMP_Text narrativeText;       

    [Header("Fade Settings")]
    public float textFadeTime = 0.8f;
    public float waitTime = 1f;

    [Header("Player Setup")]
    public PlayerController playerController;
    public Transform spawnPoint;
    public TALKINGS firstDialogue;

    [Header("Sequence Text")]
    public string line1 = "England 1900";
    public string line2 = "Z = 互动\n12345 = 选择选项";

    private bool started = false;
    private Color transparent = new Color(1, 1, 1, 0);

    void Start()
    {
        
        if (blackScreenImage != null)
        {
            blackScreenImage.color = Color.black;
            blackScreenImage.gameObject.SetActive(true);
        }

        
        if (clickPromptText != null)
        {
            clickPromptText.color = Color.white;
            clickPromptText.text = "Start";
        }

        
        if (narrativeText != null)
        {
            narrativeText.color = transparent;
            narrativeText.text = "";
        }

        // 禁用玩家
        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!started && Input.anyKeyDown)
        {
            StartSequence();
        }
    }

    public void StartSequence()
    {
        if (started) return;
        started = true;

        Debug.Log("开场开始");

        // 隐藏点击提示
        if (clickPromptText != null)
        {
            clickPromptText.DOFade(0f, 0.3f);
        }

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 等0.5秒
        yield return new WaitForSeconds(1f);

        // === 第一行文字 ===
        if (narrativeText != null)
        {
            narrativeText.text = line1;
            narrativeText.DOFade(1f, textFadeTime);
        }
        yield return new WaitForSeconds(waitTime);

        // 淡出第一行
        if (narrativeText != null)
        {
            narrativeText.DOFade(0f, textFadeTime / 2f);
        }
        yield return new WaitForSeconds(textFadeTime / 2f);

        // === 第二行文字 ===
        if (narrativeText != null)
        {
            narrativeText.text = line2;
            narrativeText.DOFade(1f, textFadeTime);
        }
        yield return new WaitForSeconds(4f); // 显示4秒

        // 淡出第二行
        if (narrativeText != null)
        {
            narrativeText.DOFade(0f, textFadeTime / 2f);
        }
        yield return new WaitForSeconds(textFadeTime / 2f);

        // === 淡出黑屏 ===
        if (blackScreenImage != null)
        {
            blackScreenImage.DOFade(0f, 0.5f);
        }
        yield return new WaitForSeconds(0.5f);

        // === 激活玩家 ===
        if (playerController != null)
        {
            playerController.gameObject.SetActive(true);

            // 传送到出生点
            if (spawnPoint != null)
            {
                playerController.transform.position = spawnPoint.position;
            }

            // 启用移动
            playerController.canMove = true;
        }

        // === 隐藏整个UI ===
        if (blackScreenImage != null)
        {
            blackScreenImage.gameObject.SetActive(false);
        }

        // === 触发第一个对话 ===
        if (firstDialogue != null)
        {
            yield return null; // 等一帧
            Debug.Log("可以触发第一个对话了");
            // 如果你想要立即开始对话：
            // firstDialogue.StartDialogue();
        }

        Debug.Log("开场结束");
    }
}