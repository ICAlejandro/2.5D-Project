using UnityEngine;

public class Delivery : Interactable
{
    [Header("Package Contents")]
    public int seedCountInside = 1;

    public override void Interact(GameObject playerObject)
    {
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        
        if (inventory != null)
        {
            inventory.AddSeeds(seedCountInside);
            Debug.Log($"Collected package! Added {seedCountInside} seed(s) to inventory.");

            PlayerHUD hud = FindFirstObjectByType<PlayerHUD>();
            if (hud != null)
            {
                hud.UpdateHUDVisuals();
            }

            // Find the zone blocker script before destroying this package object instance
            DeliveryZoneBlocker blocker = FindFirstObjectByType<DeliveryZoneBlocker>();

            Destroy(gameObject);

            // Let the engine clear out this object asset completely, then update the wall collision 
            if (blocker != null)
            {
                blocker.Invoke("EvaluateBlockerState", 0.05f);
            }
        }
    }
}