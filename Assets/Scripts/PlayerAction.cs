using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
        // Don't accept interaction inputs if a menu has already paused the game loop
        if (Time.timeScale == 0f) return;

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
        RaycastHit hit;
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;

        if (Physics.Raycast(transform.position, lookDirection, out hit, InteractionDistance, interactableLayer))
        {
            // 1. Check for the Online Shop component first
            OnlineShop onlineShopComponent = hit.collider.GetComponent<OnlineShop>();
            if (onlineShopComponent != null)
            {
                onlineShopComponent.OpenShop(gameObject);
                return; 
            }

            // 2. Check for a Farming component next
            Farming farmingPot = hit.collider.GetComponent<Farming>();
            if (farmingPot != null)
            {
                farmingPot.Interact(gameObject);
            }

            // 3. Display dialogue box if the object holds basic text data
            Interactable textData = hit.collider.GetComponent<Interactable>();
            if (textData != null)
            {
                if (masterDialogueCanvas == null || UI_TextMesh == null) return;

                UI_TextMesh.text = textData.dialogueText;
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