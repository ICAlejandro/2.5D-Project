using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Base Dialogue Settings")]
    [TextArea(3, 10)]
    public string dialogueText = "This is a placeholder interaction text.";

    public virtual void Interact(GameObject player)
    {
        PlayerAction playerAction = player.GetComponent<PlayerAction>();
        if (playerAction != null)
        {
            playerAction.DisplayDialogue(dialogueText);
        }
    }
}