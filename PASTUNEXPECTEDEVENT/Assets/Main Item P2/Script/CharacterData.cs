using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite avatar;
    public Sprite fullBody;




    public int level = 1;
    public int maxHP = 100;
    public int attack = 10;
    public int defense = 10;
}

public class CharacterSelectButton : MonoBehaviour
{
    public CharacterData character;

    public void OnClickSelectC1()
    {
        var team = FindObjectOfType<PlayerTeam>();
        var player = FindObjectOfType<PlayerController>();

        team.SetC1(character);
        player.ApplyCharacter(character);
    }

    public void OnClickSelectC2()
    {
        var team = FindObjectOfType<PlayerTeam>();
        team.SetC2(character);
    }
}
