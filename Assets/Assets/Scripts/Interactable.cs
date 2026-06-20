using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Dialogue Content")]
    [TextArea(3, 5)] 
    public string dialogueText = "Hello! I am an interactable object.";

    public void TriggerDialogue()
    {
        // Prints the text directly to your Unity Console window
        Debug.Log(dialogueText);
    }
}