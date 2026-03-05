using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TALKINGS[] allTalkings;

    public void InteractButtonPressed()
    {
        foreach (var t in allTalkings)
        {
            if (t.playerInRange && !t.isTalking)
            {
                t.StartDialogue();
                break; // 只触发一个
            }
        }
    }
}
