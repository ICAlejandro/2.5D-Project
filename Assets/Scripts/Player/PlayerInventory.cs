using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Tracking")]
    public int goldCount = 0;
    public int seedCount = 3; 
    public int cropCount = 0;
    public bool hasWater = true;

    /// <summary>Fires whenever any inventory value changes. Subscribe to this instead of polling in Update.</summary>
    public event Action OnInventoryChanged;

    // Modified: Adds a physical crop to your inventory item slot
    public void AddCrop(int amount)
    {
        cropCount += amount;
        Debug.Log("Crop added to bag! Total Crops: " + cropCount);
        OnInventoryChanged?.Invoke();
    }

    // Call this later when selling crops to a merchant for gold!
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
}