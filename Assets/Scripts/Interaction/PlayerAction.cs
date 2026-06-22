using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionDistance = 4f;
    public LayerMask interactableLayer;

    [Header("3D BoxCast Dimensions")]
    [Tooltip("How wide left-to-right (X axis) the interaction box is.")]
    public float boxWidth = 1f;
    [Tooltip("How tall up-and-down (Y axis) the interaction box is.")]
    public float boxHeight = 3f;
    [Tooltip("How thick forward-and-back (Z axis) the interaction box shape itself is.")]
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
        RaycastHit hit;
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;
        
        // Ensure the direction is purely horizontal along the ground plane
        lookDirection.y = 0f;
        lookDirection.Normalize();

        // The center half-extents of our box shape (width/2, height/2, depth/2)
        Vector3 boxHalfExtents = new Vector3(boxWidth / 2f, boxHeight / 2f, boxDepth / 2f);

        // Fire the full 3D Box forward up to 20 units to detect packages first
        if (Physics.BoxCast(transform.position, boxHalfExtents, lookDirection, out hit, transform.rotation, 20f, interactableLayer))
        {
            float distanceToTarget = hit.distance;

            // 1. Check for the Online Shop component first
            OnlineShop onlineShopComponent = hit.collider.GetComponent<OnlineShop>();
            if (onlineShopComponent != null && distanceToTarget <= InteractionDistance)
            {
                onlineShopComponent.OpenShop(gameObject);
                return; 
            }

            // 2. Check for a physical Delivery Package (Uses custom override range)
            DeliveryPackage deliveryPackage = hit.collider.GetComponent<DeliveryPackage>();
            if (deliveryPackage != null)
            {
                if (distanceToTarget <= deliveryPackage.interactionRangeOverride)
                {
                    deliveryPackage.Interact(gameObject);
                    return;
                }
                else
                {
                    return; 
                }
            }

            // 3. Check for a Farming component next
            Farming farmingPot = hit.collider.GetComponent<Farming>();
            if (farmingPot != null && distanceToTarget <= InteractionDistance)
            {
                farmingPot.Interact(gameObject);
                return;
            }

            // 4. Display dialogue box
            Interactable textData = hit.collider.GetComponent<Interactable>();
            if (textData != null && distanceToTarget <= InteractionDistance)
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

    // Draws the full 3D interaction box volume in your Scene View editor when the player is selected
    void OnDrawGizmosSelected()
    {
        Vector3 lookDirection = playerMovement != null ? playerMovement.GetLookDirection() : transform.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();
        
        Gizmos.color = Color.green;
        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        
        // Draw the wireframe box volume right where the maximum check distance lands
        Gizmos.DrawWireCube(transform.position + lookDirection * InteractionDistance, boxSize);
    }
}