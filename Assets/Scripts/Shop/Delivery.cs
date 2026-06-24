using UnityEngine;

public class Delivery : Interactable
{
    [Header("Package Contents")]
    [Tooltip("Drag the SeedData asset for the seed type this package contains.")]
    public SeedData seedType;
    public int seedCountInside = 1;

    public override void Interact(GameObject playerObject)
    {
        IInventory inventory = playerObject.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            if (seedType == null)
            {
                Debug.LogWarning("Delivery package has no SeedData assigned! Please set seedType in the Inspector.");
                return;
            }

            // AddSeeds fires OnInventoryChanged so the HUD updates automatically
            inventory.AddSeeds(seedType, seedCountInside);
            Debug.Log($"Collected package! Added {seedCountInside} {seedType.seedName}(s) to inventory.");

            DeliveryZoneBlocker blocker = FindFirstObjectByType<DeliveryZoneBlocker>();

            Destroy(gameObject);

            if (blocker != null)
                blocker.Invoke("EvaluateBlockerState", 0.05f);
        }
    }
}
