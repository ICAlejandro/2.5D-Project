using System;
using UnityEngine;

/// <summary>
/// Concrete inventory implementation. Registers itself as IInventory
/// so consumers never reference this class directly.
/// </summary>
public class PlayerInventory : MonoBehaviour, IInventory
{
    [Header("Inventory Tracking")]
    [SerializeField] private int  goldCount = 0;
    [SerializeField] private int  seedCount = 3;
    [SerializeField] private int  cropCount = 0;
    [SerializeField] private bool hasWater  = true;

    // ── IInventory properties ──────────────────────────────────────────────
    public int  GoldCount => goldCount;
    public int  SeedCount => seedCount;
    public int  CropCount => cropCount;
    public bool HasWater  => hasWater;

    // ── IInventory event ───────────────────────────────────────────────────
    public event Action OnInventoryChanged;

    // ── Unity Lifecycle ────────────────────────────────────────────────────
    void Awake()
    {
        // Register as the interface — consumers never need to know it's a PlayerInventory
        ServiceLocator.Register<IInventory>(this);
    }

    // ── IInventory mutators ────────────────────────────────────────────────
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
