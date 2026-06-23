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
            // AddSeeds fires OnInventoryChanged so the HUD updates automatically
            inventory.AddSeeds(seedCountInside);
            Debug.Log($"Collected package! Added {seedCountInside} seed(s) to inventory.");

            // Resolve from ServiceLocator instead of searching the scene
            DeliveryZoneBlocker blocker = FindFirstObjectByType<DeliveryZoneBlocker>();

            Destroy(gameObject);

            if (blocker != null)
                blocker.Invoke("EvaluateBlockerState", 0.05f);
        }
    }
}
