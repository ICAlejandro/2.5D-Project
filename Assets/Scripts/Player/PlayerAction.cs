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
    public float boxWidth = 1f;
    public float boxHeight = 3f;
    public float boxDepth = 1f;

    [Header("Master UI References")]
    public GameObject masterDialogueCanvas;
    public TextMeshProUGUI UI_TextMesh;

    private PlayerMovement playerMovement;
    private bool isDialogueOpen = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (masterDialogueCanvas != null) masterDialogueCanvas.SetActive(false);

        // Fail-safe automatic backup check for the 'Interactables' layer naming
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
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();

        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        RaycastHit hit;

        // Perform the BoxCast scan
        if (Physics.BoxCast(transform.position, boxSize / 2f, lookDirection, out hit, Quaternion.identity, InteractionDistance, interactableLayer))
        {
            Debug.Log($"BoxCast successfully hit: {hit.collider.gameObject.name}");

            // Look for the bridge component
            Interactable targetInteractable = hit.collider.GetComponentInParent<Interactable>();
            if (targetInteractable != null)
            {
                // Execute the interaction bridge without caring what script type it actually is!
                targetInteractable.Interact(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("BoxCast did not hit any objects on the specified Interactable Layer Mask.");
        }
    }

    // Public method that the base Interactable bridge can call to display text
    public void DisplayDialogue(string text)
    {
        if (masterDialogueCanvas == null || UI_TextMesh == null) return;

        UI_TextMesh.text = text;
        masterDialogueCanvas.SetActive(true);
        isDialogueOpen = true;
    }

    void CloseDialogue()
    {
        if (masterDialogueCanvas != null)
        {
            masterDialogueCanvas.SetActive(false);
            isDialogueOpen = false;
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