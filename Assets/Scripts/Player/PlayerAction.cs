using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 4f;
    public LayerMask interactableLayer;

    [Header("BoxCast Dimensions")]
    public float boxWidth  = 1f;
    public float boxHeight = 3f;
    public float boxDepth  = 1f;

    [Header("Dialogue References")]
    public GameObject      dialogueCanvas;
    public TextMeshProUGUI dialogueText;

    private PlayerMovement _playerMovement;
    private int            _dialogueOpenedFrame = -1;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (interactableLayer == 0)
        {
            int idx = LayerMask.NameToLayer("Interactables");
            if (idx != -1) interactableLayer = 1 << idx;
        }
    }

    void Update()
    {
        if (PlayerStateManager.IsState(PlayerState.Cutscene)) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (PlayerStateManager.IsFree)
            {
                if (dialogueCanvas != null && dialogueCanvas.activeSelf)
                {
                    CloseDialogue();
                }
                else
                {
                    TryInteract();
                }
            }
            else if (PlayerStateManager.IsState(PlayerState.Dialogue))
            {
                if (Time.frameCount == _dialogueOpenedFrame) return;

                OptionUI optionUI = FindFirstObjectByType<OptionUI>();
                
                if (optionUI == null || !optionUI.IsMenuOpen)
                {
                    CloseDialogue();
                }
            }
        }
    }

    private void TryInteract()
    {
        Vector3 look = _playerMovement != null
            ? _playerMovement.GetLookDirection()
            : transform.forward;

        look.y = 0f;
        look.Normalize();

        Vector3 half = new Vector3(boxWidth, boxHeight, boxDepth) * 0.5f;

        if (Physics.BoxCast(transform.position, half, look, out RaycastHit hit, Quaternion.identity, interactionDistance, interactableLayer))
        {
            Debug.Log($"BoxCast hit: {hit.collider.gameObject.name}");
            Interactable target = hit.collider.GetComponentInParent<Interactable>();
            target?.Interact(gameObject);
        }
        else
        {
            Debug.LogWarning("BoxCast hit nothing on Interactables layer.");
        }
    }

    public void DisplayDialogue(string text)
    {
        if (dialogueCanvas == null || dialogueText == null)
        {
            Debug.LogWarning("PlayerAction: dialogueCanvas or dialogueText not assigned!");
            return;
        }

        dialogueText.text = text;
        dialogueCanvas.SetActive(true);
        PlayerStateManager.SetState(PlayerState.Dialogue);
        
        _dialogueOpenedFrame = Time.frameCount;
    }

    public void CloseDialogue()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        PlayerStateManager.SetState(PlayerState.Free);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 look = _playerMovement != null
            ? _playerMovement.GetLookDirection()
            : transform.forward;

        look.y = 0f;
        look.Normalize();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position + look * interactionDistance,
            new Vector3(boxWidth, boxHeight, boxDepth)
        );
    }
}