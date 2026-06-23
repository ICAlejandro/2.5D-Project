using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Tracking")]
    public int goldCount = 0;
    public int seedCount = 3;
    public int cropCount = 0;
    public bool hasWater = true;

    public event Action OnInventoryChanged;

    void Awake()
    {
        // Register so any script can find this instantly without a scene search
        ServiceLocator.Register<PlayerInventory>(this);
    }

    public void AddCrop(int amount)
    {
        cropCount += amount;
        Debug.Log("Crop added to bag! Total Crops: " + cropCount);
        OnInventoryChanged?.Invoke();
    }

    public void SellCrops(int amount, int pricePerCrop)
    {
        if (cropCount >= amount)
        {
            cropCount -= amount;
            goldCount += (amount * pricePerCrop);
            Debug.Log($"Sold {amount} crops for {amount * pricePerCrop} gold!");
            OnInventoryChanged?.Invoke();
        }
    }

    public bool UseSeed()
    {
        if (seedCount > 0)
        {
            seedCount--;
            Debug.Log("Planted a seed. Seeds remaining: " + seedCount);
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log("No seeds left!");
        return false;
    }

    public void AddSeeds(int amount)
    {
        seedCount += amount;
        Debug.Log("Picked up " + amount + " seeds. Total: " + seedCount);
        OnInventoryChanged?.Invoke();
    }

    public void SpendGold(int amount)
    {
        goldCount -= amount;
        Debug.Log($"Spent {amount} gold. Remaining: " + goldCount);
        OnInventoryChanged?.Invoke();
    }
}
