using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Concrete inventory. Seed types are configured in the Inspector
/// via the Seed Inventory list — no coding needed to add new seed types.
/// </summary>
public class PlayerInventory : MonoBehaviour, IInventory
{
    [Header("Gold & Crops")]
    [SerializeField] private int  goldCount = 0;
    [SerializeField] private int  cropCount = 0;
    [SerializeField] private bool hasWater  = true;

    [Header("Seed Inventory")]
    [Tooltip("Add one entry per seed type. Drag a SeedData asset and set a starting amount.")]
    [SerializeField] private List<SeedInventoryEntry> seedInventory = new List<SeedInventoryEntry>();

    // ── IInventory properties ──────────────────────────────────────────────
    public int  GoldCount => goldCount;
    public int  CropCount => cropCount;
    public bool HasWater  => hasWater;
    public List<SeedInventoryEntry> SeedInventory => seedInventory;

    public event Action OnInventoryChanged;

    // ── Unity Lifecycle ────────────────────────────────────────────────────
    void Awake()
    {
        ServiceLocator.Register<IInventory>(this);
    }

    // ── Seed methods ───────────────────────────────────────────────────────
    public int GetSeedAmount(SeedData seed)
    {
        SeedInventoryEntry entry = seedInventory.Find(e => e.seedData == seed);
        return entry != null ? entry.amount : 0;
    }

    public bool UseSeed(SeedData seed)
    {
        SeedInventoryEntry entry = seedInventory.Find(e => e.seedData == seed);
        if (entry != null && entry.amount > 0)
        {
            entry.amount--;
            Debug.Log($"Used 1 {seed.seedName}. Remaining: {entry.amount}");
            OnInventoryChanged?.Invoke();
            return true;
        }

        Debug.Log($"No {seed.seedName} left!");
        return false;
    }

    public void AddSeeds(SeedData seed, int amount)
    {
        SeedInventoryEntry entry = seedInventory.Find(e => e.seedData == seed);
        if (entry != null)
        {
            entry.amount += amount;
        }
        else
        {
            // Auto-add a new entry if this seed type isn't in the list yet
            seedInventory.Add(new SeedInventoryEntry { seedData = seed, amount = amount });
        }

        Debug.Log($"Added {amount} {seed.seedName}. Total: {GetSeedAmount(seed)}");
        OnInventoryChanged?.Invoke();
    }

    // ── Other mutators ─────────────────────────────────────────────────────
    public void AddCrop(int amount)
    {
        cropCount += amount;
        Debug.Log("Crop added! Total: " + cropCount);
        OnInventoryChanged?.Invoke();
    }

    public void SellCrops(int amount, int pricePerCrop)
    {
        if (cropCount >= amount)
        {
            cropCount -= amount;
            goldCount += amount * pricePerCrop;
            Debug.Log($"Sold {amount} crop(s) for {amount * pricePerCrop} gold!");
            OnInventoryChanged?.Invoke();
        }
    }

    public void SpendGold(int amount)
    {
        goldCount -= amount;
        Debug.Log($"Spent {amount} gold. Remaining: {goldCount}");
        OnInventoryChanged?.Invoke();
    }
}
