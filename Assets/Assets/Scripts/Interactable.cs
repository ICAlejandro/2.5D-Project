using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Dialogue Content")]
    [TextArea(3, 5)] 
    public string dialogueText = "Hello! I am a simple interactable object.";

    // This script does nothing else! It just sits here holding text data.
}