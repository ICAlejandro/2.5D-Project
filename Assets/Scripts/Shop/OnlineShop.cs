using System.Collections;
using UnityEngine;

/// <summary>
/// Handles shop transaction logic only — buying seeds and selling crops.
/// UI open/close is handled separately by ShopUI.cs on the same GameObject.
/// </summary>
public class OnlineShop : Interactable
{
    [Header("Shop Balancing")]
    public int seedCost = 5;
    public int cropValue = 10;

    [Header("Delivery Settings")]
    [SerializeField] private float deliveryTimeSeconds = 3f;
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private Transform deliverySpawnPoint;

    private PlayerInventory activePlayerInventory;
    private ShopUI shopUI;

    void Start()
    {
        activePlayerInventory = ServiceLocator.Get<PlayerInventory>();
        shopUI = GetComponent<ShopUI>();
    }

    public override void Interact(GameObject player)
    {
        if (shopUI != null) shopUI.OpenShop();
    }

    public void BuySeed()
    {
        if (activePlayerInventory == null) return;

        if (activePlayerInventory.goldCount >= seedCost)
        {
            activePlayerInventory.SpendGold(seedCost);
            StartCoroutine(ProcessDeliveryRoutine(1));
            Debug.Log("Ordered 1 seed via Online Shop!");
        }
        else
        {
            Debug.Log("Not enough gold to buy a seed!");
        }
    }

    private IEnumerator ProcessDeliveryRoutine(int amountOrdered)
    {
        yield return new WaitForSecondsRealtime(deliveryTimeSeconds);

        if (packagePrefab != null && deliverySpawnPoint != null)
        {
            GameObject spawnedPackage = Instantiate(packagePrefab, deliverySpawnPoint.position, deliverySpawnPoint.rotation);
            Debug.Log("A package has arrived at the delivery point.");

            Delivery deliveryScript = spawnedPackage.GetComponent<Delivery>();
            if (deliveryScript != null)
            {
                deliveryScript.seedCountInside = amountOrdered;
            }

            DeliveryZoneBlocker blocker = deliverySpawnPoint.GetComponent<DeliveryZoneBlocker>();
            if (blocker != null)
            {
                blocker.EvaluateBlockerState();
            }
        }
    }

    public void SellCrop()
    {
        if (activePlayerInventory == null) return;

        // Use SellCrops so OnInventoryChanged fires and the HUD updates automatically
        activePlayerInventory.SellCrops(1, cropValue);
        Debug.Log("Sold 1 crop via Online Shop!");
    }
}
