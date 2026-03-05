using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEventManager : MonoBehaviour
{
    // ===============================
    // 状态存储
    // ===============================
    private Dictionary<string, bool> boolFlags = new Dictionary<string, bool>();
    private Dictionary<string, int> intValues = new Dictionary<string, int>();

    // ===============================
    // 视频系统
    // ===============================
    [Header("Bread Ending Videos")]
    public UnityEngine.Video.VideoClip breadEndingVideo;
    public UnityEngine.Video.VideoClip breadBreadEndingVideo;

    [Header("Genocide Ending Video")]
    public UnityEngine.Video.VideoClip genocideEndingVideo;

    [Header("Video Player")]
    public UnityEngine.Video.VideoPlayer videoPlayer;
    public GameObject videoScreen;

    // ===============================
    // 传送点
    // ===============================
    [Header("Teleport Targets")]
    public Transform teleport_Window1;

    // ===============================
    // 血迹（死亡可视化）
    // ===============================
    [Header("Blood Objects")]
    public GameObject blood_Butler;
    public GameObject blood_32;
    public GameObject blood_Servant;
    public GameObject blood_BreadShop;
    public GameObject blood_BadGuy;
    public GameObject blood_NPC1;
    public GameObject blood_NPC2;

    // ===============================
    // 场景事件
    // ===============================
    public GameObject Sevent1;
    public GameObject Sevent2;

    // ===============================
    // 单例
    // ===============================
    public static DialogueEventManager Instance { get; private set; }

    private bool genocideTriggered = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeVideoSystem();
    }

    // ===============================
    // 初始化视频
    // ===============================
    void InitializeVideoSystem()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.gameObject.SetActive(false);
        }

        if (videoScreen != null)
        {
            videoScreen.SetActive(false);
            RawImage rawImg = videoScreen.GetComponent<RawImage>();
            if (rawImg != null)
                rawImg.color = new Color(1, 1, 1, 0);
        }
    }

    // ===============================
    // Flag系统
    // ===============================
    public void SetBoolFlag(string flagName, bool value)
    {
        if (boolFlags.ContainsKey(flagName))
            boolFlags[flagName] = value;
        else
            boolFlags.Add(flagName, value);
    }

    public bool GetBoolFlag(string flagName)
    {
        if (boolFlags.ContainsKey(flagName))
            return boolFlags[flagName];
        return false;
    }

    public void SetIntValue(string key, int value)
    {
        if (intValues.ContainsKey(key))
            intValues[key] = value;
        else
            intValues.Add(key, value);
    }

    public int GetIntValue(string key)
    {
        if (intValues.ContainsKey(key))
            return intValues[key];
        return 0;
    }

    // ===============================
    // 传送
    // ===============================
    public void TeleportPlayer(Transform target)
    {
        if (target == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("❌ 没找到 Player（请确认玩家有 Player Tag）");
            return;
        }

        player.transform.position = target.position;
    }

    // ===============================
    // 血迹激活
    // ===============================
    void ActivateBlood(GameObject bloodObj)
    {
        if (bloodObj != null)
            bloodObj.SetActive(true);
    }

    // ===============================
    // 屠杀结局检测
    // ===============================
    bool CheckGenocideEnding()
    {
        return GetBoolFlag("ButlerDied") &&
               GetBoolFlag("32Died") &&
               GetBoolFlag("ServentDied") &&
               GetBoolFlag("BreadShopDied") &&
               GetBoolFlag("BadGuyDied") &&
               GetBoolFlag("NPC11Died") &&
               GetBoolFlag("NPC22Died");
    }

    void HandleGenocideEnding()
    {
        if (genocideTriggered) return;
        genocideTriggered = true;
        SetBoolFlag("Ending_Genocide", true);
        PlayEndingVideo(genocideEndingVideo);
    }

    void HandleBreadEnding(string type)
    {
        if (type == "Bread")
        {
            SetBoolFlag("Ending_Bread", true);
            PlayEndingVideo(breadEndingVideo);
        }
        else if (type == "BreadBread")
        {
            SetBoolFlag("Ending_BreadBread", true);
            PlayEndingVideo(breadBreadEndingVideo);
        }
    }

    // ===============================
    // 视频播放
    // ===============================
    void PlayEndingVideo(UnityEngine.Video.VideoClip clip)
    {
        if (videoPlayer == null || clip == null) return;

        videoPlayer.gameObject.SetActive(true);
        videoScreen.SetActive(true);

        RawImage rawImg = videoScreen.GetComponent<RawImage>();
        if (rawImg != null) rawImg.color = Color.white;

        Time.timeScale = 0f;
        videoPlayer.clip = clip;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnded;
    }

    void OnVideoEnded(UnityEngine.Video.VideoPlayer vp)
    {
        Time.timeScale = 1f;
        videoScreen.SetActive(false);

        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);
        vp.loopPointReached -= OnVideoEnded;

        QuitGameNow();
    }

    void QuitGameNow()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===============================
    // 对话事件入口
    // ===============================
    public void OnDialogueEvent(string eventName)
    {
        switch (eventName)
        {
            // ===== 物品 =====
            case "Knife": SetBoolFlag("HasKnife", true); break;
            case "KeepKnife": SetBoolFlag("HasKnife", false); break;
            case "HaveScissor": SetBoolFlag("HasScissors", true); break;
            case "HaveTissue": SetBoolFlag("HasTissue", true); break;
            case "HaveWine": SetBoolFlag("HasWine", true); break;

            // ===== 角色状态 =====
            case "32ishere": SetBoolFlag("32IsTogether", true); break;
            case "Jacket": SetBoolFlag("WearingJacket", true); break;
            case "Handk": SetBoolFlag("WearingHandk", true); break;
            case "BKnowKill": SetBoolFlag("ButlerKnowsKill", true); break;
            case "BKnowMing": SetBoolFlag("ButlerKnowsMing", true); break;

            // ===== 任务 =====
            case "MissionEat":
                SetBoolFlag("MissionEat", true);
                SetBoolFlag("IsHungry", true);
                break;

            case "NotHungry":
                SetBoolFlag("IsHungry", false);
                if (GetBoolFlag("MissionEat"))
                    SetBoolFlag("MissionEatComplete", true);
                break;

            // ===== 场景切换 =====
            case "ServantTalk":
                SetBoolFlag("ST", false);
                Sevent1.SetActive(false);
                Sevent2.SetActive(true);
                break;

            // ===== 传送 =====
            case "Window1":
                TeleportPlayer(teleport_Window1);
                break;

            // ===== 死亡（血迹+flag）=====
            case "BDied":
                SetBoolFlag("ButlerDied", true);
                ActivateBlood(blood_Butler);
                break;

            case "MDied":
                SetBoolFlag("32Died", true);
                ActivateBlood(blood_32);
                break;

            case "SDied":
                SetBoolFlag("ServentDied", true);
                ActivateBlood(blood_Servant);
                break;

            case "BSDied":
                SetBoolFlag("BreadShopDied", true);
                ActivateBlood(blood_BreadShop);
                break;

            case "BGDied":
                SetBoolFlag("BadGuyDied", true);
                ActivateBlood(blood_BadGuy);
                break;

            case "NPC1Died":
                SetBoolFlag("NPC11Died", true);
                ActivateBlood(blood_NPC1);
                break;

            case "NPC2Died":
                SetBoolFlag("NPC22Died", true);
                ActivateBlood(blood_NPC2);
                break;

            // ===== 结局 =====
            case "BreadEnding":
                HandleBreadEnding("Bread");
                break;

            case "BreadBreadEnding":
                HandleBreadEnding("BreadBread");
                break;

            case "GenocideEnding":
                if (CheckGenocideEnding())
                    HandleGenocideEnding();

                break;








// ===== Tissue 系统 =====
case "OT":
{
    int count = GetIntValue("TissueCount");
    count++;
    SetIntValue("TissueCount", count);

    // 自动同步 HasTissue
    SetBoolFlag("HasTissue", count > 0);
}
break;

case "OT-":
{
    int count = GetIntValue("TissueCount");
    count = Mathf.Max(0, count - 1);
    SetIntValue("TissueCount", count);

    // 自动同步 HasTissue
    SetBoolFlag("HasTissue", count > 0);
}
break;

case "OT?":
{
    int count = GetIntValue("TissueCount");
    SetBoolFlag("HasTissue", count > 0);
}
break;




















        }
    }
}
