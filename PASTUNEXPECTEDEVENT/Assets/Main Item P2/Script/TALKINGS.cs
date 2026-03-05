using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[System.Serializable]

public class DialogueOption
{
    public string optionText;
    public int jumpToStateIndex;
    public int jumpToElementIndex;

    // === 新增：条件判断字段 ===
    [Tooltip("需要满足的布尔标志名称（如：HasKnife）")]
    public string requiredFlag = "";

    [Tooltip("需要满足的标志值")]
    public bool requiredFlagValue = true;
}

[System.Serializable]
public class DialogueElement
{
    [TextArea(2, 6)]
    public string rawText;
    public bool destroyNPCWhenFinished = false;
    public bool hideNPCWhenFinished = false;
    public string customEventName = "";
    public bool isChoicePoint = false;
    public DialogueOption[] choices;
}

[System.Serializable]
public class DialogueState
{
    public string stateName = "1";
    public DialogueElement[] dialogueElements;
}

public class TALKINGS : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image leftImage;
    public Image rightImage;
    public CanvasGroup leftCanvas;
    public CanvasGroup rightCanvas;
    public RectTransform leftRect;
    public RectTransform rightRect;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public TMP_Text[] choiceButtons;
    public Button[] choiceButtonComponents;

[Header("Mobile Input")]
public Button interactButton;

    [Header("Prompt")]
    public TMP_Text interactPrompt;

    [Header("Audio")]
    public AudioSource talkingAudio;
    public AudioSource InteractSound;

    [Header("Player")]
    public PlayerController player;

    [Header("Data")]
    public DialogueState[] dialogueStates;

    [Header("Settings")]
    public bool triggerOnce = false;

    private int dialogueTriggerCount = 0;
    private DialogueElement[] currentDialogueArray;
    private string currentStateName = "";
    private DialogueOption[] currentChoices;

    private bool hasTriggered = false;
[HideInInspector] public bool playerInRange = false;
[HideInInspector] public bool isTalking = false;

    private bool isTyping = false;
    private Coroutine typingCoroutine = null;
    private string currentFullText = "";
    private int currentIndex = 0;

    private bool currentDestroyCommand = false;
    private bool currentHideCommand = false;
    private string currentEventName = "";

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.alpha = 0f;
        if (leftCanvas != null) leftCanvas.alpha = 0f;
        if (rightCanvas != null) rightCanvas.alpha = 0f;
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);

        if (dialogueStates == null || dialogueStates.Length == 0)
        {
            Debug.LogWarning("TALKINGS: dialogueStates 未设置！", this);
        }
    if (interactButton != null)
        interactButton.onClick.AddListener(() =>
        {
            if (playerInRange && !isTalking)
                StartDialogue();
            else if (isTalking)
                NextLineOrSkipTyping();
        });
    }

    private void IDK()
    {
        if (isTalking) InteractSound.Play();
    }




private void NextLineOrSkipTyping()
{
    if (isTyping)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        dialogueText.text = currentFullText;
        CancelInvoke(nameof(AutoNext));
    }
    else
    {
        NextLine();
    }
}









    void Update()
    {
        if (choicePanel != null && choicePanel.activeSelf && currentChoices != null)
        {
            // 支持1-5号选项按键
            if (Input.GetKeyDown(KeyCode.Alpha1) && currentChoices.Length > 0)
                OnChoiceSelected(currentChoices[0]);
            else if (Input.GetKeyDown(KeyCode.Alpha2) && currentChoices.Length > 1)
                OnChoiceSelected(currentChoices[1]);
            else if (Input.GetKeyDown(KeyCode.Alpha3) && currentChoices.Length > 2)
                OnChoiceSelected(currentChoices[2]);
            else if (Input.GetKeyDown(KeyCode.Alpha4) && currentChoices.Length > 3)
                OnChoiceSelected(currentChoices[3]);
            else if (Input.GetKeyDown(KeyCode.Alpha5) && currentChoices.Length > 4) // 新增第5个选项
                OnChoiceSelected(currentChoices[4]);
            return;
        }

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(playerInRange && !isTalking);

if (playerInRange && !isTalking && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E)))
    StartDialogue();


        if (isTalking && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E)))
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                isTyping = false;
                dialogueText.text = currentFullText;
                CancelInvoke(nameof(AutoNext));
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue()
    {
        if (triggerOnce && hasTriggered) return;

        int stateIndex = Mathf.Min(dialogueTriggerCount, dialogueStates.Length - 1);
        if (dialogueStates.Length == 0)
        {
            Debug.LogError("没有设置任何对话状态！", this);
            return;
        }

        currentStateName = dialogueStates[stateIndex].stateName;
        currentDialogueArray = dialogueStates[stateIndex].dialogueElements;
        Debug.Log($"开始对话，触发次数: {dialogueTriggerCount}, 使用状态: {currentStateName}");
        dialogueTriggerCount++;

        hasTriggered = true;
        if (player != null) player.canMove = false;
        isTalking = true;
        currentIndex = 0;
        if (dialoguePanel != null) dialoguePanel.DOFade(1f, 0.18f);
        DisplayElementSafely(currentIndex);
    }

    private void NextLine()
    {
        CancelInvoke(nameof(AutoNext));
        currentIndex++;
        if (currentDialogueArray == null || currentIndex >= currentDialogueArray.Length)
        {
            EndDialogue();
            return;
        }
        DisplayElementSafely(currentIndex);
    }

    private void AutoNext()
    {
        if (!isTyping) NextLine();
    }

    private void EndDialogue()
    {
        CancelInvoke(nameof(AutoNext));
        if (player != null) player.canMove = true;
        isTalking = false;

        if (dialoguePanel != null) dialoguePanel.DOFade(0f, 0.18f);
        if (leftCanvas != null) leftCanvas.DOFade(0f, 0.18f);
        if (rightCanvas != null) rightCanvas.DOFade(0f, 0.18f);
        if (dialogueText != null) dialogueText.text = "";
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

        Debug.Log($"对话结束，下次将使用状态索引: {Mathf.Min(dialogueTriggerCount, dialogueStates.Length - 1)}");
        ExecuteDialogueCommands();
    }

    private void DisplayElementSafely(int index)
    {
        if (currentDialogueArray == null || currentDialogueArray.Length == 0) return;
        if (index < 0 || index >= currentDialogueArray.Length) return;

        CancelInvoke(nameof(AutoNext));
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        DisplayElement(currentDialogueArray[index]);
    }

    private void DisplayElement(DialogueElement element)
    {
        if (element.isChoicePoint && element.choices != null && element.choices.Length > 0)
        {
            if (!string.IsNullOrEmpty(element.rawText))
            {
                string choiceText = element.rawText;
                string namePattern = @"\[Name(.*?)\]";
                Match nameMatch = Regex.Match(choiceText, namePattern);
                if (nameMatch.Success)
                {
                    string extractedName = nameMatch.Groups[1].Value;
                    choiceText = choiceText.Replace(nameMatch.Value, ""); // ← 清除标签
                    if (nameText != null) nameText.text = extractedName;
                }

                // ===== 新增：用统一的 ParseTag 再清理一次其他标签 =====
                // 确保所有 [LImg]、[RImg] 等标签都被清除
                choiceText = CleanAllTags(choiceText);

                if (dialogueText != null) dialogueText.text = choiceText;
            }
            ShowChoices(element.choices);
            return;
        }

        string text = element.rawText ?? "";

        string ParseTag(ref string txt, string tag)
        {
            string pattern = $@"\[{tag}(.*?)\]";
            Match m = Regex.Match(txt, pattern);
            if (m.Success)
            {
                string value = m.Groups[1].Value.Trim();
                txt = txt.Replace(m.Value, "");
                return value;
            }
            return "";
        }


        string name = ParseTag(ref text, "Name");
        string lImg = ParseTag(ref text, "LImg");
        string rImg = ParseTag(ref text, "RImg");
        string lEase = ParseTag(ref text, "LEase");
        string rEase = ParseTag(ref text, "REase");


        string lt = "";
        if (text.Contains("[LT]")) { lt = "L"; text = text.Replace("[LT]", ""); }
        if (text.Contains("[RT]")) { lt = "R"; text = text.Replace("[RT]", ""); }
        if (text.Contains("[AT]")) { lt = "A"; text = text.Replace("[AT]", ""); }

        string colorHex = "#FFFFFF";
        string colorTag = ParseTag(ref text, "#");
        if (!string.IsNullOrEmpty(colorTag)) colorHex = "#" + colorTag;

        string timeTag = ParseTag(ref text, "Time");
        float timeSec = 0f;
        if (!string.IsNullOrEmpty(timeTag)) float.TryParse(timeTag, out timeSec);

        string speedTag = ParseTag(ref text, "Speed");
        float speed = 0.03f;
        if (!string.IsNullOrEmpty(speedTag)) float.TryParse(speedTag, out speed);

        if (nameText != null) nameText.text = name;
        if (dialogueText != null)
        {
            if (ColorUtility.TryParseHtmlString(colorHex, out Color col)) dialogueText.color = col;
            else dialogueText.color = Color.white;
        }

        LoadSprite(lImg, leftImage, leftCanvas);
        LoadSprite(rImg, rightImage, rightCanvas);
        ApplyBrightness(lt);
        ApplyEase(leftRect, leftCanvas, lEase);
        ApplyEase(rightRect, rightCanvas, rEase);

        currentFullText = text;
        typingCoroutine = StartCoroutine(TypeTextCoroutineSafe(text, speed));

        if (talkingAudio != null) talkingAudio.Play();
        if (timeSec > 0f)
        {
            CancelInvoke(nameof(AutoNext));
            Invoke(nameof(AutoNext), timeSec);
        }

        currentDestroyCommand = element.destroyNPCWhenFinished;
        currentHideCommand = element.hideNPCWhenFinished;
        currentEventName = element.customEventName;
    }

private void ShowChoices(DialogueOption[] choices)
{
    if (choicePanel == null || choiceButtons == null)
    {
        Debug.LogError("选项UI未设置！");
        if (choices.Length > 0) OnChoiceSelected(choices[0]);
        return;
    }

    // 1. 过滤符合条件的选项
    List<DialogueOption> validChoices = new List<DialogueOption>();
    foreach (var choice in choices)
    {
        bool shouldShow = true;

        if (!string.IsNullOrEmpty(choice.requiredFlag))
        {
            bool currentFlagValue = DialogueEventManager.Instance.GetBoolFlag(choice.requiredFlag);
            shouldShow = (currentFlagValue == choice.requiredFlagValue);
        }

        if (shouldShow)
            validChoices.Add(choice);
    }

    // 2. 如果没有符合条件的选项，直接自动选择第一个
    if (validChoices.Count == 0)
    {
        Debug.LogWarning("没有符合条件的对话选项，自动选择第一个");
        OnChoiceSelected(choices[0]);
        return;
    }

    // 3. 保存当前选择数组
    currentChoices = validChoices.ToArray();
    if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);

    // 4. 显示按钮并绑定事件
    for (int i = 0; i < choiceButtons.Length; i++)
    {
        if (i < validChoices.Count)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].text = $"{validChoices[i].optionText}.{i + 1}";
            int index = i;

            // 清除旧监听
            if (choiceButtonComponents.Length > i)
            {
                choiceButtonComponents[i].onClick.RemoveAllListeners();
                choiceButtonComponents[i].onClick.AddListener(() => OnChoiceSelected(validChoices[index]));
            }
        }
        else
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    // 5. 显示选项面板
    choicePanel.SetActive(true);
    CancelInvoke(nameof(AutoNext));
}


    private void OnChoiceSelected(DialogueOption selectedChoice)
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        currentChoices = null;

        // 注意：这里跳转使用的是原始 choices 数组的索引，不是过滤后的
        // 保持原有的跳转逻辑
        int targetState = Mathf.Clamp(selectedChoice.jumpToStateIndex, 0, dialogueStates.Length - 1);
        currentStateName = dialogueStates[targetState].stateName;
        currentDialogueArray = dialogueStates[targetState].dialogueElements;
        currentIndex = Mathf.Clamp(selectedChoice.jumpToElementIndex, 0, currentDialogueArray.Length - 1);
        DisplayElementSafely(currentIndex);
    }

    private void LoadSprite(string imgName, Image img, CanvasGroup cg)
    {
        if (img == null) return;
        bool loaded = false;

        if (!string.IsNullOrEmpty(imgName))
        {
            Sprite s = Resources.Load<Sprite>("CharacterReaction/" + imgName);
#if UNITY_EDITOR
            if (s == null)
            {
                string path = "Assets/Main Item Assets.P2.Art.CharacterReaction/" + imgName;
                s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path + ".png");
                if (s == null) s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path + ".jpg");
            }
#endif
            if (s != null)
            {
                img.sprite = s;
                loaded = true;
            }
            else Debug.LogWarning($"Sprite not found: {imgName}");
        }

        if (cg != null)
        {
            cg.alpha = loaded ? 1f : 0f;
            cg.gameObject.SetActive(loaded);
        }
    }

    private void ApplyBrightness(string lt)
    {
        if (leftCanvas == null || rightCanvas == null) return;
        lt = lt.ToUpper();
        if (lt == "L")
        {
            leftCanvas.DOFade(1f, 0.12f);
            rightCanvas.DOFade(0.32f, 0.12f);
        }
        else if (lt == "R")
        {
            leftCanvas.DOFade(0.32f, 0.12f);
            rightCanvas.DOFade(1f, 0.12f);
        }
        else
        {
            leftCanvas.DOFade(1f, 0.12f);
            rightCanvas.DOFade(1f, 0.12f);
        }
    }

    private void ApplyEase(RectTransform rt, CanvasGroup cg, string easeTag)
    {
        if (string.IsNullOrEmpty(easeTag) || rt == null) return;
        easeTag = easeTag.Trim().ToUpper();
        Sequence seq = DOTween.Sequence();
        switch (easeTag)
        {
            case "YQ":
                Vector2 orig = rt.anchoredPosition;
                rt.anchoredPosition = orig + new Vector2(0, -22f);
                seq.Append(rt.DOAnchorPos(orig, 0.12f).SetEase(Ease.OutQuad));
                break;
            case "SHAKE":
                seq.Append(rt.DOShakeAnchorPos(0.35f, new Vector2(12f, 6f), 12, 90f));
                break;
            case "SB":
                rt.localScale = Vector3.one * 0.6f;
                seq.Append(rt.DOScale(1.05f, 0.09f)).Append(rt.DOScale(1f, 0.06f));
                break;
            case "SS":
                rt.localScale = Vector3.one * 1.2f;
                seq.Append(rt.DOScale(1f, 0.12f));
                break;
        }
        if (cg != null) seq.Join(cg.DOFade(1f, 0.08f));
    }

    private IEnumerator TypeTextCoroutineSafe(string fullText, float delay)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";
        foreach (var c in fullText)
        {
            if (dialogueText != null) dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
        typingCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
        IDK();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isTalking) EndDialogue();
        }
    }

    public void ResetDialogueCount()
    {
        dialogueTriggerCount = 0;
        Debug.Log($"{gameObject.name} 对话次数已重置", this);
    }

    public string GetCurrentStateName()
    {
        return currentStateName;
    }

    // 新增的标签清理方法
    private string CleanAllTags(string text)
    {
        // 清除所有格式为 [XXX...] 的标签
        string pattern = @"\[[^\]]*\]";
        return Regex.Replace(text, pattern, "");
    }


    private void ExecuteDialogueCommands()
    {






        // 1. 先执行自定义事件
        if (!string.IsNullOrEmpty(currentEventName))
        {
            Debug.Log($"触发自定义事件: {currentEventName}");

            DialogueEventManager eventManager = FindObjectOfType<DialogueEventManager>();
            if (eventManager != null)
            {
                eventManager.OnDialogueEvent(currentEventName);
            }
            else
            {
                Debug.LogError($"[TALKINGS] 错误：尝试触发事件 '{currentEventName}'，但在场景中未找到 DialogueEventManager！");
            }

            currentEventName = "";
        }

        // 2. 再处理销毁/隐藏（注意：这会导致脚本停止运行）
        if (currentDestroyCommand)
        {
            Debug.Log($"{gameObject.name} 因对话指令被销毁");
            Destroy(this.gameObject, 0.1f);
            return; // 销毁后会停止后续代码执行
        }

         if (currentHideCommand)
         {
             Debug.Log($"{gameObject.name} 因对话指令被隐藏");
             this.gameObject.SetActive(false);
             Collider2D col = GetComponent<Collider2D>();
             if (col != null) col.enabled = false;
         }

        // 3. 重置标志（在销毁前重置，或者在销毁后由其他方式处理）
        if (!currentDestroyCommand) // 如果不是销毁才重置，因为销毁后不需要重置
        {
            currentDestroyCommand = false;
            //currentHideCommand = false;
        }
    }
}