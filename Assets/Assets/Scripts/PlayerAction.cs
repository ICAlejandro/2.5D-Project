using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionDistance = 4f; 
    public LayerMask interactableLayer;     

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        RaycastHit hit;
        Vector3 lookDirection = playerMovement.GetLookDirection();
        
        if (Physics.Raycast(transform.position, lookDirection, out hit, InteractionDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.TriggerDialogue();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerMovement != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, playerMovement.GetLookDirection() * InteractionDistance);
        }
    }
}