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
        
        lookDirection.y = 0f;
        lookDirection.Normalize();

        Vector3 boxHalfExtents = new Vector3(boxWidth / 2f, boxHeight / 2f, boxDepth / 2f);

        if (Physics.BoxCast(transform.position, boxHalfExtents, lookDirection, out hit, transform.rotation, InteractionDistance, interactableLayer))
        {
            float distanceToTarget = hit.distance;

            OnlineShop onlineShopComponent = hit.collider.GetComponent<OnlineShop>();
            if (onlineShopComponent != null && distanceToTarget <= InteractionDistance)
            {
                onlineShopComponent.OpenShop(gameObject);
                return; 
            }

            DeliveryPackage deliveryPackage = hit.collider.GetComponent<DeliveryPackage>();
            if (deliveryPackage != null && distanceToTarget <= InteractionDistance)
            {
                deliveryPackage.Interact(gameObject);
                return;
            }

            Farming farmingPot = hit.collider.GetComponent<Farming>();
            if (farmingPot != null && distanceToTarget <= InteractionDistance)
            {
                farmingPot.Interact(gameObject);
                return;
            }

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