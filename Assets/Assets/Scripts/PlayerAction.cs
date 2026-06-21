using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Added to talk to the new Input System

public class PlayerAction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionDistance = 4f; 
    public LayerMask interactableLayer;     

    [Header("Master UI References")]
    public GameObject masterDialogueCanvas; 
    public TextMeshProUGUI UI_TextMesh;     

    private PlayerMovement playerMovement;
    private bool isDialogueOpen = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (masterDialogueCanvas != null) masterDialogueCanvas.SetActive(false);
    }

    void Update()
    {
        // New Input System check for the Enter/Return key being pressed this frame
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isDialogueOpen)
            {
                CloseDialogue();
            }
            else
            {
                TryInteract();
            }
        }
    }

    void TryInteract()
    {
        if (playerMovement == null) return;

        RaycastHit hit;
        Vector3 lookDirection = playerMovement.GetLookDirection();
        
        if (Physics.Raycast(transform.position, lookDirection, out hit, InteractionDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (masterDialogueCanvas == null || UI_TextMesh == null) return;

                UI_TextMesh.text = interactable.dialogueText;
                masterDialogueCanvas.SetActive(true);
                isDialogueOpen = true;
            }
        }
    }

    void CloseDialogue()
    {
        if (masterDialogueCanvas != null)
        {
            masterDialogueCanvas.SetActive(false);
            isDialogueOpen = false;
        }
    }
}