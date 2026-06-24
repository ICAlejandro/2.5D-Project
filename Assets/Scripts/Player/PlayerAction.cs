using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionDistance = 4f;

    [Tooltip("Make sure either 'Interactable' or 'Interactables' is selected here!")]
    public LayerMask interactableLayer;

    [Header("3D BoxCast Dimensions")]
    public float boxWidth  = 1f;
    public float boxHeight = 3f;
    public float boxDepth  = 1f;

    [Header("Dialogue Canvas References")]
    public GameObject      dialogueCanvas;
    public TextMeshProUGUI dialogueText;

    private PlayerMovement playerMovement;
    private bool   isDialogueOpen = false;
    private Action _onDialogueClosed;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);

        if (interactableLayer == 0)
        {
            int defaultLayerIndex = LayerMask.NameToLayer("Interactables");
            if (defaultLayerIndex != -1)
            {
                interactableLayer = 1 << defaultLayerIndex;
                Debug.Log("PlayerAction automatically targeted the 'Interactables' physics layer mask.");
            }
        }
    }

    void Update()
    {
        // Allow closing dialogue during Dialogue state, block everything else
        if (PlayerStateManager.IsState(PlayerState.Cutscene)) return;

        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isDialogueOpen) CloseDialogue();
            else if (PlayerStateManager.IsFree) TryInteract();
        }
    }

    void TryInteract()
    {
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();

        Vector3    boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        RaycastHit hit;

        if (Physics.BoxCast(transform.position, boxSize / 2f, lookDirection, out hit, Quaternion.identity, InteractionDistance, interactableLayer))
        {
            Debug.Log($"BoxCast successfully hit: {hit.collider.gameObject.name}");
            Interactable targetInteractable = hit.collider.GetComponentInParent<Interactable>();
            if (targetInteractable != null)
                targetInteractable.Interact(gameObject);
        }
        else
        {
            Debug.LogWarning("BoxCast did not hit any objects on the specified Interactable Layer Mask.");
        }
    }

    /// <summary>
    /// Shows dialogue text. Optional onClose callback fires when player dismisses it.
    /// Automatically locks player movement via PlayerStateManager.
    /// </summary>
    public void DisplayDialogue(string text, Action onClose = null)
    {
        if (dialogueCanvas == null || dialogueText == null) return;

        _onDialogueClosed = onClose;
        dialogueText.text = text;
        dialogueCanvas.SetActive(true);
        isDialogueOpen = true;

        // Lock player movement — state is Dialogue, not a cutscene
        PlayerStateManager.SetState(PlayerState.Dialogue);
    }

    void CloseDialogue()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
            isDialogueOpen = false;

            // Only return to Free if OptionUI isn't also open
            if (!isDialogueOpen)
                PlayerStateManager.SetState(PlayerState.Free);

            Action callback = _onDialogueClosed;
            _onDialogueClosed = null;
            callback?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();

        Gizmos.color = Color.green;
        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        Gizmos.DrawWireCube(transform.position + lookDirection * InteractionDistance, boxSize);
    }
}
