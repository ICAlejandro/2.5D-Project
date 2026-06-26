using UnityEngine;
using TMPro;

public class PlayerAction : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag your InputReader ScriptableObject asset here.")]
    public InputReader inputReader;

    [Header("Interaction Settings")]
    public float     interactionDistance = 4f;
    public LayerMask interactableLayer;

    [Header("BoxCast Dimensions")]
    public float boxWidth  = 1f;
    public float boxHeight = 3f;
    public float boxDepth  = 1f;

    [Header("Dialogue References")]
    public GameObject      dialogueCanvas;
    public TextMeshProUGUI dialogueText;

    private PlayerMovement _playerMovement;
    private int            _lastDialogueFrame = -1;

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

        if (inputReader != null)
        {
            inputReader.OnInteractEvent += HandleInteract;
            inputReader.OnConfirmEvent  += HandleConfirm;
        }
    }

    void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.OnInteractEvent -= HandleInteract;
            inputReader.OnConfirmEvent  -= HandleConfirm;
        }
    }

    private void HandleInteract()
    {
        if (PlayerStateManager.IsState(PlayerState.Cutscene)) return;
        if (Time.frameCount == _lastDialogueFrame) return;

        if (PlayerStateManager.IsFree)
            TryInteract();
    }

    private void HandleConfirm()
    {
        if (PlayerStateManager.IsState(PlayerState.Cutscene)) return;
        if (Time.frameCount == _lastDialogueFrame) return;

        if (!PlayerStateManager.IsState(PlayerState.Dialogue)) return;

        OptionUI optionUI = FindFirstObjectByType<OptionUI>();
        if (optionUI == null || !optionUI.IsMenuOpen)
            CloseDialogue();
    }

    private void TryInteract()
    {
        Vector3 look = _playerMovement != null
            ? _playerMovement.GetLookDirection()
            : transform.forward;

        look.y = 0f;
        look.Normalize();

        Vector3 half = new Vector3(boxWidth, boxHeight, boxDepth) * 0.5f;

        if (Physics.BoxCast(transform.position, half, look, out RaycastHit hit,
                            Quaternion.identity, interactionDistance, interactableLayer))
        {
            Interactable target = hit.collider.GetComponentInParent<Interactable>();
            target?.Interact(gameObject);
        }
    }

    public void DisplayDialogue(string text)
    {
        if (dialogueCanvas == null || dialogueText == null) return;

        dialogueText.text = text;
        dialogueCanvas.SetActive(true);
        _lastDialogueFrame = Time.frameCount;

        PlayerStateManager.SetState(PlayerState.Dialogue);
    }

    public void CloseDialogue()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        _lastDialogueFrame = Time.frameCount;

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