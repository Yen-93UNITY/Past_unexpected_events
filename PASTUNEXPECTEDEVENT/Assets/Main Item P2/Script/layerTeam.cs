using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    public CharacterData slot1;   // C1
    public CharacterData slot2;   // C2

    public CharacterData current; // ?????????????=slot1?

    // ? C1
    public void SetC1(CharacterData c)
    {
        slot1 = c;
        current = slot1;
    }

    // ? C2
    public void SetC2(CharacterData c)
    {
        slot2 = c;
    }
}
