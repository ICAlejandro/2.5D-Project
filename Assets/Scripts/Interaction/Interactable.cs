using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Base Dialogue Settings")]
    [TextArea(3, 10)]
    public string dialogueText = "This is a placeholder interaction text.";

    // The bridge method. 'virtual' allows other scripts to override this function with custom logic.
    public virtual void Interact(GameObject player)
    {
        // Default behavior: Tell the player script to fire up the generic dialogue window
        PlayerAction playerAction = player.GetComponent<PlayerAction>();
        if (playerAction != null)
        {
            playerAction.DisplayDialogue(dialogueText);
        }
    }
}