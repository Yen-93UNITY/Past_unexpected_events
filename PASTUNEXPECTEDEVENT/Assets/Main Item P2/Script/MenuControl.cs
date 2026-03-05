using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [Header("=== Panels ===")]
    public GameObject panelMenu;
    public GameObject panelCharacter;
    public GameObject panelInventory;
    public GameObject panelSetting;
    public GameObject panelClassSelection;

    [Header("=== Character Display (C1 / C2) ===")]
    public Image imgC1;
    public Image imgC2;

    [Header("=== Current Characters ===")]
    public CharacterData character1;
    public CharacterData character2;

    void Start()
    {
        RefreshCharacterUI();
    }

    // 更新 UI 显示
    public void RefreshCharacterUI()
    {
        if (character1 != null)
            imgC1.sprite = character1.avatar;

        if (character2 != null)
            imgC2.sprite = character2.avatar;
    }

    // 打开菜单
    public void OpenMenu()
    {
        panelMenu.SetActive(true);
    }

    // 关闭菜单
    public void CloseMenu()
    {
        panelMenu.SetActive(false);
    }

    // === Menu Buttons ===
    public void OpenCharacterPanel()
    {
        panelMenu.SetActive(false);
        panelCharacter.SetActive(true);
    }

    public void OpenInventoryPanel()
    {
        panelMenu.SetActive(false);
        panelInventory.SetActive(true);
    }

    public void OpenSettingPanel()
    {
        panelMenu.SetActive(false);
        panelSetting.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // === Class Selection ===
    public void OpenClassSelection()
    {
        panelClassSelection.SetActive(true);
    }

    public void CloseClassSelection()
    {
        panelClassSelection.SetActive(false);
    }

    // 角色更换函数
    public void SetCharacter1(CharacterData newChar)
    {
        character1 = newChar;
        RefreshCharacterUI();
    }

    public void SetCharacter2(CharacterData newChar)
    {
        character2 = newChar;
        RefreshCharacterUI();
    }
}
